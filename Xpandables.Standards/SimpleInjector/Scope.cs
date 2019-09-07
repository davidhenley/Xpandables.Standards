﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SimpleInjector.Advanced;
    using SimpleInjector.Internals;
    using SimpleInjector.Lifestyles;

    /// <summary>Implements a cache for <see cref="ScopedLifestyle"/> implementations.</summary>
    /// <remarks>
    /// <see cref="Scope"/> is thread-safe can be used over multiple threads concurrently, but note that the
    /// cached instances might not be thread-safe.
    /// </remarks>
    public class Scope : ApiObject, IDisposable, IServiceProvider
    {
        private const int MaximumDisposeRecursion = 100;

        private readonly object syncRoot = new object();
        private readonly ScopeManager? manager;

        private IDictionary? items;
        private Dictionary<Registration, object>? cachedInstances;
        private List<Action>? scopeEndActions;
        private List<IDisposable>? disposables;
        private DisposeState state;
        private int recursionDuringDisposalCounter;

        /// <summary>Initializes a new instance of the <see cref="Scope"/> class.</summary>
        [Obsolete("Use the overloaded Scope(Container) constructor instead. " +
            "Will be treated as an error from version 5.0. Will be removed in version 6.0.",
            error: false)]
        public Scope()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Scope"/> class.</summary>
        /// <param name="container">The container instance that the scope belongs to.</param>
        public Scope(Container container)
        {
            Requires.IsNotNull(container, nameof(container));

            Container = container;
        }

        internal Scope(Container container, ScopeManager manager, Scope? parentScope) : this(container)
        {
            Requires.IsNotNull(manager, nameof(manager));

            ParentScope = parentScope;
            this.manager = manager;
        }

        private enum DisposeState
        {
            Alive,
            Disposing,
            Disposed
        }

        /// <summary>Gets the container instance that this scope belongs to.</summary>
        /// <value>The <see cref="Container"/> instance.</value>
        public Container? Container { get; }

        internal bool Disposed => state == DisposeState.Disposed;

        internal Scope? ParentScope { get; }

        /// <summary>Gets an instance of the given <typeparamref name="TService"/> for the current scope.</summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>An instance of the given service type.</returns>
        public TService GetInstance<TService>() where TService : class
        {
            return (TService)GetInstance(typeof(TService));
        }

        /// <summary>Gets an instance of the given <paramref name="serviceType" /> for the current scope.</summary>
        /// <param name="serviceType">The type of the service to resolve.</param>
        /// <returns>An instance of the given service type.</returns>
        public object GetInstance(Type serviceType)
        {
            Requires.IsNotNull(serviceType, nameof(serviceType));

            if (Container == null)
            {
                throw new InvalidOperationException(
                    "This method can only be called on Scope instances that are related to a Container. " +
                    "Please use the overloaded constructor of Scope create an instance with a Container.");
            }

            Scope? originalScope = Container.CurrentThreadResolveScope;

            try
            {
                Container.CurrentThreadResolveScope = this;
                return Container.GetInstance(serviceType);
            }
            finally
            {
                Container.CurrentThreadResolveScope = originalScope;
            }
        }

        /// <summary>
        /// Allows registering an <paramref name="action"/> delegate that will be called when the scope ends,
        /// but before the scope disposes any instances.
        /// </summary>
        /// <remarks>
        /// During the call to <see cref="Scope.Dispose()"/> all registered <see cref="Action"/> delegates are
        /// processed in the order of registration. Do note that registered actions <b>are not guaranteed
        /// to run</b>. In case an exception is thrown during the call to <see cref="Dispose()"/>, the
        /// <see cref="Scope"/> will stop running any actions that might not have been invoked at that point.
        /// Instances that are registered for disposal using <see cref="RegisterForDisposal"/> on the other
        /// hand, are guaranteed to be disposed. Note that registered actions won't be invoked during a call
        /// to <see cref="Container.Verify()" />.
        /// </remarks>
        /// <param name="action">The delegate to run when the scope ends.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference
        /// (Nothing in VB).</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the scope has been disposed.</exception>
        public virtual void WhenScopeEnds(Action action)
        {
            Requires.IsNotNull(action, nameof(action));

            lock (syncRoot)
            {
                RequiresInstanceNotDisposed();
                PreventCyclicDependenciesDuringDisposal();

                if (scopeEndActions == null)
                {
                    scopeEndActions = new List<Action>();
                }

                scopeEndActions.Add(action);
            }
        }

        /// <summary>
        /// Adds the <paramref name="disposable"/> to the list of items that will get disposed when the
        /// scope ends.
        /// </summary>
        /// <remarks>
        /// Instances that are registered for disposal, will be disposed in opposite order of registration and
        /// they are guaranteed to be disposed when <see cref="Scope.Dispose()"/> is called (even when
        /// exceptions are thrown). This mimics the behavior of the C# and VB <code>using</code> statements,
        /// where the <see cref="IDisposable.Dispose"/> method is called inside the <code>finally</code> block.
        /// </remarks>
        /// <param name="disposable">The instance that should be disposed when the scope ends.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is a null reference
        /// (Nothing in VB).</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the scope has been disposed.</exception>
        public void RegisterForDisposal(IDisposable disposable)
        {
            Requires.IsNotNull(disposable, nameof(disposable));

            lock (syncRoot)
            {
                RequiresInstanceNotDisposed();
                PreventCyclicDependenciesDuringDisposal();

                RegisterForDisposalInternal(disposable);
            }
        }

        /// <summary>
        /// Retrieves an item from the scope stored by the given <paramref name="key"/> or null when no
        /// item is stored by that key.
        /// </summary>
        /// <remarks>
        /// <b>Thread-safety:</b> Calls to this method are thread-safe, but users should take proper
        /// percussions when they call both <b>GetItem</b> and <see cref="SetItem"/>.
        /// </remarks>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <returns>The stored item or null (Nothing in VB).</returns>
        /// <exception cref="ArgumentNullException">Thrown when one of the supplied arguments is a null
        /// reference (Nothing in VB).</exception>
        public object? GetItem(object key)
        {
            Requires.IsNotNull(key, nameof(key));

            lock (syncRoot)
            {
                return items?[key];
            }
        }

        /// <summary>Stores an item by the given <paramref name="key"/> in the scope.</summary>
        /// <remarks>
        /// <b>Thread-safety:</b> Calls to this method are thread-safe, but users should take proper
        /// percussions when they call both <see cref="GetItem"/> and <b>SetItem</b>.
        /// </remarks>
        /// <param name="key">The key of the item to insert or override.</param>
        /// <param name="item">The actual item. May be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when paramref name="key"/> is a null reference
        /// (Nothing in VB).</exception>
        public void SetItem(object key, object? item)
        {
            Requires.IsNotNull(key, nameof(key));

            lock (syncRoot)
            {
                if (items is null)
                {
                    items = new Dictionary<object, object?>(capacity: 1);
                }

                if (item is null)
                {
                    items.Remove(key);
                }
                else
                {
                    items[key] = item;
                }
            }
        }

        /// <summary>
        /// Returns the list of <see cref="IDisposable"/> instances that will be disposed of when this <see cref="Scope"/>
        /// instance is being disposed. The list contains scoped instances that are cached in this <see cref="Scope"/> instance,
        /// and instances explicitly registered for disposal using <see cref="RegisterForDisposal"/>. The instances are returned
        /// in order of creation/registration. When <see cref="Dispose()">Scope.Dispose</see> is called, the scope will ensure
        /// <see cref="IDisposable.Dispose"/> is called on each instance in this list. The instance will be disposed in opposite
        /// order as they appear in the list.
        /// </summary>
        /// <returns>The list of <see cref="IDisposable"/> instances that will be disposed of when this <see cref="Scope"/>
        /// instance is being disposed.</returns>
        public IDisposable[] GetDisposables()
        {
            lock (syncRoot)
            {
                RequiresInstanceNotDisposed();

                if (disposables == null)
                {
                    return Helpers.Array<IDisposable>.Empty;
                }

                return disposables.ToArray();
            }
        }

        /// <summary>Releases all instances that are cached by the <see cref="Scope"/> object.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType -or- null if there is no service object of type
        /// <paramref name="serviceType"/>.</returns>
        object? IServiceProvider.GetService(Type serviceType)
        {
            Requires.IsNotNull(serviceType, nameof(serviceType));

            IServiceProvider? provider = Container;

            if (provider is null)
            {
                throw new InvalidOperationException(
                    "This method can only be called on Scope instances that are related to a Container. " +
                    "Please use the overloaded constructor of Scope create an instance with a Container.");
            }

            Scope? originalScope = Container!.CurrentThreadResolveScope;

            try
            {
                Container.CurrentThreadResolveScope = this;
                return provider.GetService(serviceType);
            }
            finally
            {
                Container.CurrentThreadResolveScope = originalScope;
            }
        }

        internal static TImplementation GetInstance<TImplementation>(
            ScopedRegistration<TImplementation> registration, Scope? scope)
            where TImplementation : class
        {
            if (scope == null)
            {
                return Scope.GetScopelessInstance(registration);
            }

            return scope.GetInstanceInternal(registration);
        }

        internal T GetOrSetItem<T>(object key, Func<Container, object, T> valueFactory)
        {
            lock (syncRoot)
            {
                if (items is null)
                {
                    items = new Dictionary<object, object>(capacity: 1);
                }

                object? item = items[key];

                if (item is null)
                {
                    items[key] = item = valueFactory(Container!, key);
                }

                return (T)item!;
            }
        }

        /// <summary>
        /// Releases all instances that are cached by the <see cref="Scope"/> object.
        /// </summary>
        /// <param name="disposing">False when only unmanaged resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // We completely block the Dispose method from running in parallel, because there's all kinds
                // of state that needs to be read/written, such as this.state, this.disposables, and
                // this.scopeEndActions. Making this thread-safe with smaller granular locks will be much
                // harder and simply not necessarily, since Dispose should normally only be called from one thread.
                lock (syncRoot)
                {
                    if (state != DisposeState.Alive)
                    {
                        // Either this instance is already disposed, or a different thread is currently
                        // disposing it. We can break out immediately.
                        return;
                    }

                    state = DisposeState.Disposing;

                    try
                    {
                        DisposeRecursively();
                    }
                    finally
                    {
                        state = DisposeState.Disposed;

                        // Remove all references, so we won't hold on to created instances even if the
                        // scope accidentally keeps referenced. This prevents leaking memory.
                        cachedInstances = null;
                        scopeEndActions = null;
                        disposables = null;
                        items = null;

                        manager?.RemoveScope(this);
                    }
                }
            }
        }

        private void DisposeRecursively(bool operatingInException = false)
        {
            if (disposables == null && (operatingInException || scopeEndActions == null))
            {
                return;
            }

            recursionDuringDisposalCounter++;

            try
            {
                if (!operatingInException)
                {
                    while (scopeEndActions != null)
                    {
                        ExecuteAllRegisteredEndScopeActions();
                        recursionDuringDisposalCounter++;
                    }
                }

                DisposeAllRegisteredDisposables();
            }
            catch
            {
                // When an exception is thrown during disposing, we immediately stop executing all
                // registered actions, but continue disposing all cached instances. This simulates the
                // behavior of a using statement, where the actions are part of the try-block.
                bool firstException = !operatingInException;

                if (firstException)
                {
                    // We must reset the counter here, because even if a recursion was detected in one of the
                    // actions, we still want to try disposing all instances.
                    recursionDuringDisposalCounter = 0;
                    operatingInException = true;
                }

                throw;
            }
            finally
            {
                // We must break out of the recursion when we reach MaxRecursion, because not doing so
                // could cause a stack overflow.
                if (recursionDuringDisposalCounter <= MaximumDisposeRecursion)
                {
                    DisposeRecursively(operatingInException);
                }
            }
        }

        private void ExecuteAllRegisteredEndScopeActions()
        {
            if (scopeEndActions != null)
            {
                var actions = scopeEndActions;

                scopeEndActions = null;

                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }

        private static TImplementation GetScopelessInstance<TImplementation>(
            ScopedRegistration<TImplementation> registration)
            where TImplementation : class
        {
            if (registration.Container.IsVerifying)
            {
                return registration.Container.VerificationScope!.GetInstanceInternal(registration);
            }

            throw new ActivationException(
                StringResources.TheServiceIsRequestedOutsideTheContextOfAScopedLifestyle(
                    typeof(TImplementation),
                    registration.Lifestyle));
        }

        private TImplementation GetInstanceInternal<TImplementation>(
            ScopedRegistration<TImplementation> registration)
            where TImplementation : class
        {
            lock (syncRoot)
            {
                RequiresInstanceNotDisposed();

                bool cacheIsEmpty = cachedInstances == null;

                if (cachedInstances == null)
                {
                    cachedInstances =
                        new Dictionary<Registration, object>(ReferenceEqualityComparer<Registration>.Instance);
                }

                return !cacheIsEmpty && cachedInstances.TryGetValue(registration, out object? instance)
                    ? (TImplementation)instance
                    : CreateAndCacheInstance(registration, cachedInstances);
            }
        }

        private TImplementation CreateAndCacheInstance<TImplementation>(
            ScopedRegistration<TImplementation> registration, Dictionary<Registration, object> cache)
            where TImplementation : class
        {
            // registration.BuildExpression has been called, and InstanceCreate thus been initialized.
            Func<TImplementation> instanceCreator = registration.InstanceCreator!;

            TImplementation instance = instanceCreator.Invoke();

            cache[registration] = instance;

            if (instance is IDisposable disposable && !registration.SuppressDisposal)
            {
                RegisterForDisposalInternal(disposable);
            }

            return instance;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void RegisterForDisposalInternal(IDisposable disposable)
        {
            if (disposables == null)
            {
                disposables = new List<IDisposable>(capacity: 4);
            }

            disposables.Add(disposable);
        }

        private void DisposeAllRegisteredDisposables()
        {
            if (disposables != null)
            {
                var instances = disposables;

                disposables = null;

                DisposeInstancesInReverseOrder(instances);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void RequiresInstanceNotDisposed()
        {
            if (state == DisposeState.Disposed)
            {
                ThrowObjectDisposedException();
            }
        }

        private void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void PreventCyclicDependenciesDuringDisposal()
        {
            if (recursionDuringDisposalCounter > MaximumDisposeRecursion)
            {
                ThrowRecursionException();
            }
        }

        private static void ThrowRecursionException() =>
            throw new InvalidOperationException(StringResources.RecursiveInstanceRegistrationDetected());

        // This method simulates the behavior of a set of nested 'using' statements: It ensures that dispose
        // is called on each element, even if a previous instance threw an exception.
        private static void DisposeInstancesInReverseOrder(
            List<IDisposable> disposables, int startingAsIndex = int.MinValue)
        {
            if (startingAsIndex == int.MinValue)
            {
                startingAsIndex = disposables.Count - 1;
            }

            try
            {
                while (startingAsIndex >= 0)
                {
                    disposables[startingAsIndex].Dispose();

                    startingAsIndex--;
                }
            }
            finally
            {
                if (startingAsIndex >= 0)
                {
                    DisposeInstancesInReverseOrder(disposables, startingAsIndex - 1);
                }
            }
        }
    }
}