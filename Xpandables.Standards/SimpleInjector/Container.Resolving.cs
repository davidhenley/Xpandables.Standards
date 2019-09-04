// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using SimpleInjector.Internals;
    using SimpleInjector.Lifestyles;

    /// <summary>Methods for resolving instances.</summary>
    public partial class Container : IServiceProvider
    {
        private static readonly MethodInfo EnumerableToArrayMethod =
            typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));

        private static readonly MethodInfo EnumerableToListMethod =
            typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        private readonly Dictionary<Type, LazyEx<InstanceProducer>> resolveUnregisteredTypeRegistrations =
            new Dictionary<Type, LazyEx<InstanceProducer>>();

        private readonly Dictionary<Type, InstanceProducer?> emptyAndRedirectedCollectionRegistrationCache =
            new Dictionary<Type, InstanceProducer?>();

        // Cache for producers that are resolved as root type
        // PERF: The rootProducerCache uses a special equality comparer that does a quicker lookup of types.
        // PERF: This collection is updated by replacing the complete collection.
        private Dictionary<Type, InstanceProducer?> rootProducerCache =
            new Dictionary<Type, InstanceProducer?>(ReferenceEqualityComparer<Type>.Instance);

        private enum MutableCollectionType { Array, List }

        /// <summary>Gets an instance of the given <typeparamref name="TService"/>.</summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="ActivationException">Thrown when there are errors resolving the service instance.</exception>
        public TService GetInstance<TService>() where TService : class
        {
            ThrowWhenDisposed();
            LockContainer();

            // Performance optimization: This if check is a duplicate to save a call to GetInstanceForType.
            if (!rootProducerCache.TryGetValue(typeof(TService), out InstanceProducer? instanceProducer))
            {
                return (TService)GetInstanceForRootType<TService>();
            }

            if (instanceProducer is null)
            {
                ThrowMissingInstanceProducerException(typeof(TService));
            }

            return (TService)instanceProducer!.GetInstance();
        }

        /// <summary>Gets an instance of the given <paramref name="serviceType"/>.</summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="ActivationException">Thrown when there are errors resolving the service instance.</exception>
        public object GetInstance(Type serviceType)
        {
            ThrowWhenDisposed();
            LockContainer();

            if (!rootProducerCache.TryGetValue(serviceType, out InstanceProducer? instanceProducer))
            {
                return GetInstanceForRootType(serviceType);
            }

            if (instanceProducer is null)
            {
                ThrowMissingInstanceProducerException(serviceType);
            }

            return instanceProducer!.GetInstance();
        }

        /// <summary>
        /// Gets all instances of the given <typeparamref name="TService"/> currently registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <returns>A sequence of instances of the requested TService.</returns>
        /// <exception cref="ActivationException">Thrown when there are errors resolving the service instance.</exception>
        public IEnumerable<TService> GetAllInstances<TService>()
            where TService : class
        {
            return GetInstance<IEnumerable<TService>>();
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/> currently registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <returns>A sequence of instances of the requested serviceType.</returns>
        /// <exception cref="ActivationException">Thrown when there are errors resolving the service instance.</exception>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            Requires.IsNotNull(serviceType, nameof(serviceType));

            Type collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            return (IEnumerable<object>)GetInstance(collectionType);
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType.  -or- null if there is no service object of type
        /// <paramref name="serviceType"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "Users are not expected to inherit from this class and override this implementation.")]
        object? IServiceProvider.GetService(Type serviceType)
        {
            Requires.IsNotNull(serviceType, nameof(serviceType));
            ThrowWhenDisposed();
            LockContainer();

            if (!rootProducerCache.TryGetValue(serviceType, out InstanceProducer? instanceProducer))
            {
                instanceProducer = GetRegistration(serviceType);
            }

            return instanceProducer?.IsValid == true
                ? instanceProducer.GetInstance()
                : null;
        }

        /// <summary>
        /// Gets the <see cref="InstanceProducer"/> for the given <paramref name="serviceType"/>. When no
        /// registration exists, the container will try creating a new producer. A producer can be created
        /// when the type is a concrete reference type, there is an <see cref="ResolveUnregisteredType"/>
        /// event registered that acts on that type, or when the service type is an <see cref="IEnumerable{T}"/>.
        /// Otherwise <b>null</b> (Nothing in VB) is returned.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A call to this method locks the container. New registrations can't be made after a call to this
        /// method.
        /// </para>
        /// <para>
        /// <b>Note:</b> This method is <i>not</i> guaranteed to always return the same
        /// <see cref="InstanceProducer"/> instance for a given <see cref="System.Type"/>. It will however either
        /// always return <b>null</b> or always return a producer that is able to return the expected instance.
        /// </para>
        /// </remarks>
        /// <param name="serviceType">The <see cref="System.Type"/> that the returned instance producer should produce.</param>
        /// <returns>An <see cref="InstanceProducer"/> or <b>null</b> (Nothing in VB).</returns>
        public InstanceProducer? GetRegistration(Type serviceType)
        {
            return GetRegistration(serviceType, throwOnFailure: false);
        }

        /// <summary>
        /// Gets the <see cref="InstanceProducer"/> for the given <paramref name="serviceType"/>. When no
        /// registration exists, the container will try creating a new producer. A producer can be created
        /// when the type is a concrete reference type, there is an <see cref="ResolveUnregisteredType"/>
        /// event registered that acts on that type, or when the service type is an <see cref="IEnumerable{T}"/>.
        /// Otherwise <b>null</b> (Nothing in VB) is returned, or an exception is throw when
        /// <paramref name="throwOnFailure"/> is set to <b>true</b>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A call to this method locks the container. New registrations can't be made after a call to this
        /// method.
        /// </para>
        /// <para>
        /// <b>Note:</b> This method is <i>not</i> guaranteed to always return the same
        /// <see cref="InstanceProducer"/> instance for a given <see cref="System.Type"/>. It will however either
        /// always return <b>null</b> or always return a producer that is able to return the expected instance.
        /// </para>
        /// </remarks>
        /// <param name="serviceType">The <see cref="System.Type"/> that the returned instance producer should produce.</param>
        /// <param name="throwOnFailure">The indication whether the method should return null or throw
        /// an exception when the type is not registered.</param>
        /// <returns>An <see cref="InstanceProducer"/> or <b>null</b> (Nothing in VB).</returns>
        //// Yippie, we broke a framework design guideline rule here :-).
        //// 7.1 DO NOT have public members that can either throw or not based on some option.
        public InstanceProducer? GetRegistration(Type serviceType, bool throwOnFailure)
        {
            Requires.IsNotNull(serviceType, nameof(serviceType));

            // GetRegistration might lock the container, but only when not-explicitly made registrations are
            // requested.
            ThrowWhenDisposed();

            if (!rootProducerCache.TryGetValue(serviceType, out InstanceProducer? producer))
            {
                producer = GetExplicitlyRegisteredInstanceProducer(serviceType, InjectionConsumerInfo.Root);

                if (producer == null)
                {
                    // The producer is created implicitly. This forces us to lock the container.
                    // Such implicit registration could be done through in numberous ways (such as
                    // through unregistered type resolution, or because the type is concrete). Being able to
                    // make registrations after such call, could lead to unexpected behavior, which is why
                    // locking the container makes most sense.
                    // We even lock when the producer is null, because unregistered type resolution events may
                    // have been invoked.
                    LockContainer();

                    producer = GetRegistrationEvenIfInvalid(
                        serviceType, InjectionConsumerInfo.Root, autoCreateConcreteTypes: true);
                }

                // Add the producer, even when it's null.
                AppendRootInstanceProducer(serviceType, producer);
            }

            bool producerIsValid = producer?.IsValid == true;

            if (!producerIsValid && throwOnFailure)
            {
                ThrowInvalidRegistrationException(serviceType, producer);
            }

            // Prevent returning invalid producers
            return producerIsValid ? producer : null;
        }

        internal Action<object> GetInitializer(Type implementationType, Registration context)
        {
            return GetInitializer<object>(implementationType, context);
        }

        internal InstanceProducer? GetRegistrationEvenIfInvalid(
            Type serviceType, InjectionConsumerInfo consumer, bool autoCreateConcreteTypes = true)
        {
            if (serviceType.ContainsGenericParameters)
            {
                throw new ArgumentException(
                    StringResources.OpenGenericTypesCanNotBeResolved(serviceType), nameof(serviceType));
            }

            // This Func<T> is a bit ugly, but does save us a lot of duplicate code.
            Func<InstanceProducer?> buildProducer =
                () => BuildInstanceProducerForType(serviceType, consumer, autoCreateConcreteTypes);

            return GetInstanceProducerForType(serviceType, consumer, buildProducer);
        }

        internal bool IsConcreteConstructableType(Type concreteType) =>
            Options.IsConstructableType(concreteType, out string? errorMesssage);

        private Action<T> GetInitializer<T>(Type implementationType, Registration context)
        {
            Action<T>[] initializersForType = GetInstanceInitializersFor<T>(implementationType, context);

            if (initializersForType.Length <= 1)
            {
                return initializersForType.FirstOrDefault();
            }
            else
            {
                return obj =>
                {
                    for (int index = 0; index < initializersForType.Length; index++)
                    {
                        initializersForType[index](obj);
                    }
                };
            }
        }

        private InstanceProducer? GetInstanceProducerForType<TService>(InjectionConsumerInfo context)
            where TService : class
        {
            // This generic overload allows retrieving types that are internal inside a sandbox.
            return GetInstanceProducerForType(
                typeof(TService),
                context,
                () => BuildInstanceProducerForType<TService>(context));
        }

        private InstanceProducer? GetInstanceProducerForType(Type serviceType, InjectionConsumerInfo context)
        {
            return GetInstanceProducerForType(
                serviceType, context, () => BuildInstanceProducerForType(serviceType, context));
        }

        private object GetInstanceForRootType<TService>() where TService : class
        {
            InstanceProducer? producer = GetInstanceProducerForType<TService>(InjectionConsumerInfo.Root);
            AppendRootInstanceProducer(typeof(TService), producer);
            return GetInstanceFromProducer(producer, typeof(TService));
        }

        private object GetInstanceForRootType(Type serviceType)
        {
            if (serviceType.ContainsGenericParameters)
            {
                throw new ArgumentException(StringResources.OpenGenericTypesCanNotBeResolved(serviceType),
                    nameof(serviceType));
            }

            InstanceProducer? producer = GetInstanceProducerForType(serviceType, InjectionConsumerInfo.Root);
            AppendRootInstanceProducer(serviceType, producer);
            return GetInstanceFromProducer(producer, serviceType);
        }

        private object GetInstanceFromProducer(InstanceProducer? instanceProducer, Type serviceType)
        {
            if (instanceProducer is null)
            {
                ThrowMissingInstanceProducerException(serviceType);
            }

            // We create the instance AFTER registering the instance producer. Registering the producer after
            // creating an instance, could make us loose all registrations that are done by GetInstance. This
            // will not have any functional effects, but can result in a performance penalty.
            return instanceProducer!.GetInstance();
        }

        private InstanceProducer? BuildInstanceProducerForType<TService>(InjectionConsumerInfo context)
            where TService : class
        {
            return BuildInstanceProducerForType(typeof(TService),
                () => TryBuildInstanceProducerForConcreteUnregisteredType<TService>(context));
        }

        private InstanceProducer? BuildInstanceProducerForType(
            Type serviceType, InjectionConsumerInfo context, bool autoCreateConcreteTypes = true)
        {
            var tryBuildInstanceProducerForConcrete = autoCreateConcreteTypes && !serviceType.IsAbstract
                ? () => TryBuildInstanceProducerForConcreteUnregisteredType(serviceType, context)
                : (Func<InstanceProducer?>)(() => null);

            return BuildInstanceProducerForType(serviceType, tryBuildInstanceProducerForConcrete);
        }

        private InstanceProducer? BuildInstanceProducerForType(
            Type serviceType, Func<InstanceProducer?> tryBuildInstanceProducerForConcreteType)
        {
            return
                TryBuildInstanceProducerThroughUnregisteredTypeResolution(serviceType) ??
                TryBuildInstanceProducerForCollectionType(serviceType) ??
                tryBuildInstanceProducerForConcreteType();
        }

        private InstanceProducer? TryBuildInstanceProducerForCollectionType(Type serviceType)
        {
            return
                TryBuildInstanceProducerForMutableCollection(serviceType) ??
                TryBuildInstanceProducerForStream(serviceType);
        }

        // Instead of wrapping the complete method in a lock, we lock inside the individual methods. We
        // don't want to hold a lock while calling back into user code, because who knows what the user
        // is doing there. We don't want a dead lock.
        private InstanceProducer? TryBuildInstanceProducerThroughUnregisteredTypeResolution(Type serviceType) =>
            TryGetInstanceProducerForUnregisteredTypeResolutionFromCache(serviceType)
            ?? TryGetInstanceProducerThroughResolveUnregisteredTypeEvent(serviceType);

        private InstanceProducer? TryGetInstanceProducerForUnregisteredTypeResolutionFromCache(Type serviceType)
        {
            lock (resolveUnregisteredTypeRegistrations)
            {
                return resolveUnregisteredTypeRegistrations.ContainsKey(serviceType)
                    ? resolveUnregisteredTypeRegistrations[serviceType].Value
                    : null;
            }
        }

        private InstanceProducer? TryGetInstanceProducerThroughResolveUnregisteredTypeEvent(Type serviceType)
        {
            if (resolveUnregisteredType == null)
            {
                return null;
            }

            var e = new UnregisteredTypeEventArgs(serviceType);

            resolveUnregisteredType(this, e);

            return e.Handled
                ? TryGetProducerFromUnregisteredTypeResolutionCacheOrAdd(e)
                : null;
        }

        private InstanceProducer? TryGetProducerFromUnregisteredTypeResolutionCacheOrAdd(
            UnregisteredTypeEventArgs e)
        {
            lock (resolveUnregisteredTypeRegistrations)
            {
                if (resolveUnregisteredTypeRegistrations.ContainsKey(e.UnregisteredServiceType))
                {
                    // This line will only get hit, in case a different thread came here first.
                    return resolveUnregisteredTypeRegistrations[e.UnregisteredServiceType].Value;
                }

                var registration = e.Registration ?? new ExpressionRegistration(e.Expression!, this);

                // By creating the InstanceProducer after checking the dictionary, we prevent the producer
                // from being created twice when multiple threads are running. Having the same duplicate
                // producer can cause a torn lifestyle warning in the container.
                var producer = new InstanceProducer(e.UnregisteredServiceType, registration);

                resolveUnregisteredTypeRegistrations[e.UnregisteredServiceType] =
                    Helpers.ToLazy(producer);

                return producer;
            }
        }

        private InstanceProducer? TryBuildInstanceProducerForMutableCollection(Type serviceType)
        {
            if (serviceType.IsArray)
            {
                return BuildInstanceProducerForMutableCollectionType(
                    serviceType,
                    serviceType.GetElementType(),
                    MutableCollectionType.Array);
            }
            else if (typeof(List<>).IsGenericTypeDefinitionOf(serviceType))
            {
                return BuildInstanceProducerForMutableCollectionType(
                    serviceType,
                    serviceType.GetGenericArguments().FirstOrDefault(),
                    MutableCollectionType.List);
            }
            else
            {
                return null;
            }
        }

        private InstanceProducer? BuildInstanceProducerForMutableCollectionType(
            Type serviceType, Type elementType, MutableCollectionType type)
        {
            // We don't auto-register collections for ambiguous types.
            if (Types.IsAmbiguousOrValueType(elementType))
            {
                return null;
            }

            // GetAllInstances locks the container
            if (GetAllInstances(elementType) is IContainerControlledCollection)
            {
                return BuildMutableCollectionProducerFromControlledCollection(
                    serviceType, elementType, type);
            }
            else
            {
                return BuildMutableCollectionProducerFromUncontrolledCollection(
                    serviceType, elementType);
            }
        }

        private InstanceProducer BuildMutableCollectionProducerFromControlledCollection(
            Type serviceType, Type elementType, MutableCollectionType collectionType)
        {
            Expression expression =
                BuildMutableCollectionExpressionFromControlledCollection(serviceType, elementType);

            // Technically, we could determine the longest lifestyle out of the elements of the collection,
            // instead of using Transient here. This would make it less likely for the user to get false
            // positive Lifestyle Mismatch warnings. Problem with that is that trying to retrieve the
            // longest lifestyle might cause the array to be cached in a way that is incorrect, because
            // who knows what kind of lifestyles the used created.
            Registration registration =
                new ExpressionRegistration(expression, serviceType, Lifestyle.Transient, this);

            return new InstanceProducer(serviceType, registration)
            {
                IsContainerAutoRegistered = !GetAllInstances(elementType).Any()
            };
        }

        private Expression BuildMutableCollectionExpressionFromControlledCollection(
            Type serviceType, Type elementType)
        {
            var streamExpression = Expression.Constant(
                value: GetAllInstances(elementType),
                type: typeof(IEnumerable<>).MakeGenericType(elementType));

            if (serviceType.IsArray)
            {
                // builds: Enumerable.ToArray(collection)
                return Expression.Call(
                    EnumerableToArrayMethod.MakeGenericMethod(elementType),
                    streamExpression);
            }
            else
            {
                // builds: new List<T>(collection)
                var listConstructor = typeof(List<>).MakeGenericType(elementType)
                    .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });

                return Expression.New(listConstructor, streamExpression);
            }
        }

        private InstanceProducer BuildMutableCollectionProducerFromUncontrolledCollection(
            Type serviceType, Type elementType)
        {
            InstanceProducer? enumerableProducer = GetRegistration(
                typeof(IEnumerable<>).MakeGenericType(elementType), throwOnFailure: true);
            Expression enumerableExpression = enumerableProducer!.BuildExpression();

            var expression = Expression.Call(
                method: serviceType.IsArray
                    ? EnumerableToArrayMethod.MakeGenericMethod(elementType)
                    : EnumerableToListMethod.MakeGenericMethod(elementType),
                arg0: enumerableExpression);

            Registration registration =
                new ExpressionRegistration(expression, serviceType, Lifestyle.Transient, this);

            return new InstanceProducer(serviceType, registration)
            {
                IsContainerAutoRegistered = true
            };
        }

        private InstanceProducer? TryBuildInstanceProducerForStream(Type serviceType)
        {
            if (!Types.IsGenericCollectionType(serviceType))
            {
                return null;
            }

            // We don't auto-register collections for ambiguous types.
            if (Types.IsAmbiguousOrValueType(serviceType.GetGenericArguments()[0]))
            {
                return null;
            }

            lock (emptyAndRedirectedCollectionRegistrationCache)
            {
                // We need to cache these generated producers, to prevent getting duplicate producers; which
                // will cause (incorrect) diagnostic warnings.
                if (!emptyAndRedirectedCollectionRegistrationCache.TryGetValue(
                    serviceType, out InstanceProducer? producer))
                {
                    // This call might lock the container
                    producer = TryBuildStreamInstanceProducer(serviceType);

                    emptyAndRedirectedCollectionRegistrationCache[serviceType] = producer;
                }

                return producer;
            }
        }

        private InstanceProducer? TryBuildStreamInstanceProducer(Type collectionType)
        {
            if (typeof(IEnumerable<>).IsGenericTypeDefinitionOf(collectionType))
            {
                return null;
            }

            Type elementType = collectionType.GetGenericArguments()[0];

            var stream = GetAllInstances(elementType) as IContainerControlledCollection;

            if (stream == null)
            {
                return null;
            }

            Registration registration = stream.CreateRegistration(collectionType, this);

            return new InstanceProducer(collectionType, registration)
            {
                IsContainerAutoRegistered = !((IEnumerable<object>)stream).Any()
            };
        }

        private InstanceProducer BuildEmptyCollectionInstanceProducerForEnumerable(Type enumerableType)
        {
            Type elementType = enumerableType.GetGenericArguments()[0];

            var collection = ControlledCollectionHelper.CreateContainerControlledCollection(elementType, this);

            var registration = new ExpressionRegistration(Expression.Constant(collection, enumerableType), this);

            // Producers for ExpressionRegistration are normally ignored as external producer, but in this
            // case the empty collection producer should pop up in the list of GetCurrentRegistrations().
            return new InstanceProducer(enumerableType, registration, registerExternalProducer: true);
        }

        private InstanceProducer? TryBuildInstanceProducerForConcreteUnregisteredType<TConcrete>(
            InjectionConsumerInfo context)
            where TConcrete : class
        {
            if (Options.ResolveUnregisteredConcreteTypes
                && IsConcreteConstructableType(typeof(TConcrete)))
            {
                Func<InstanceProducer> instanceProducerBuilder = () =>
                {
                    var registration = SelectionBasedLifestyle.CreateRegistration<TConcrete>(this);

                    return BuildInstanceProducerForConcreteUnregisteredType(typeof(TConcrete), registration);
                };

                return GetOrBuildInstanceProducerForConcreteUnregisteredType(
                    typeof(TConcrete), instanceProducerBuilder);
            }

            return null;
        }

        private InstanceProducer? TryBuildInstanceProducerForConcreteUnregisteredType(
            Type type, InjectionConsumerInfo context)
        {
            if (!Options.ResolveUnregisteredConcreteTypes
                || type.IsAbstract
                || type.IsValueType
                || type.ContainsGenericParameters
                || !IsConcreteConstructableType(type))
            {
                return null;
            }

            Func<InstanceProducer> instanceProducerBuilder = () =>
            {
                var registration = SelectionBasedLifestyle.CreateRegistration(type, this);

                return BuildInstanceProducerForConcreteUnregisteredType(type, registration);
            };

            return GetOrBuildInstanceProducerForConcreteUnregisteredType(type, instanceProducerBuilder);
        }

        private InstanceProducer GetOrBuildInstanceProducerForConcreteUnregisteredType(Type concreteType,
            Func<InstanceProducer> instanceProducerBuilder)
        {
            // We need to take a lock here to make sure that we never create multiple InstanceProducer
            // instances for the same concrete type, which is a problem when the LifestyleSelectionBehavior
            // has been overridden. For instance in case the overridden behavior returns a Singleton lifestyle,
            // but the concrete type is requested concurrently over multiple threads, not taking the lock
            // could cause two InstanceProducers to be created, which might cause two instances from being
            // created.
            lock (unregisteredConcreteTypeInstanceProducers)
            {
                InstanceProducer producer;

                if (!unregisteredConcreteTypeInstanceProducers.TryGetValue(concreteType, out producer))
                {
                    producer = instanceProducerBuilder.Invoke();

                    unregisteredConcreteTypeInstanceProducers[concreteType] = producer;
                }

                return producer;
            }
        }

        private static InstanceProducer BuildInstanceProducerForConcreteUnregisteredType(Type concreteType,
            Registration registration)
        {
            var producer = new InstanceProducer(concreteType, registration);

            producer.EnsureTypeWillBeExplicitlyVerified();

            // Flag that this producer is resolved by the container or using unregistered type resolution.
            producer.IsContainerAutoRegistered = true;

            return producer;
        }

        // We're registering a service type after 'locking down' the container here and that means that the
        // type is added to a copy of the registrations dictionary and the original replaced with a new one.
        // This 'reference swapping' is thread-safe, but can result in types disappearing again from the
        // registrations when multiple threads simultaneously add different types. This however, does not
        // result in a consistency problem, because the missing type will be again added later. This type of
        // swapping safes us from using locks.
        private void AppendRootInstanceProducer(Type serviceType, InstanceProducer? rootProducer)
        {
            var snapshotCopy = rootProducerCache.MakeCopy();

            // This registration might already exist if it was added made by another thread. That's why we
            // need to use the indexer, instead of Add.
            snapshotCopy[serviceType] = rootProducer;

            // Prevent the compiler, JIT, and processor to reorder these statements to prevent the instance
            // producer from being added after the snapshot has been made accessible to other threads.
            // This is important on architectures with a weak memory model (such as ARM).

            Thread.MemoryBarrier();

            // Replace the original with the new version that includes the serviceType (make snapshot public).
            rootProducerCache = snapshotCopy;

            if (rootProducer != null)
            {
                RemoveExternalProducer(rootProducer);
            }
        }

        private void ThrowInvalidRegistrationException(Type serviceType, InstanceProducer? producer)
        {
            if (producer != null)
            {
                // Exception is never null in this context.
                throw producer.Exception!;
            }
            else
            {
                ThrowMissingInstanceProducerException(serviceType);
            }
        }

        private void ThrowMissingInstanceProducerException(Type serviceType)
        {
            if (Types.IsConcreteConstructableType(serviceType))
            {
                ThrowNotConstructableException(serviceType);
            }

            throw new ActivationException(StringResources.NoRegistrationForTypeFound(
                serviceType,
                HasRegistrations,
                ContainsOneToOneRegistrationForCollectionType(serviceType),
                ContainsCollectionRegistrationFor(serviceType),
                GetNonGenericDecoratorsThatWereSkippedDuringBatchRegistration(serviceType),
                GetLookalikesForMissingType(serviceType)));
        }

        private bool ContainsOneToOneRegistrationForCollectionType(Type collectionServiceType) =>
            Types.IsGenericCollectionType(collectionServiceType) &&
                ContainsExplicitRegistrationFor(collectionServiceType.GetGenericArguments()[0]);

        // NOTE: MakeGenericType will fail for IEnumerable<T> when T is a pointer.
        private bool ContainsCollectionRegistrationFor(Type serviceType) =>
            !Types.IsGenericCollectionType(serviceType) && !serviceType.IsPointer &&
                ContainsExplicitRegistrationFor(typeof(IEnumerable<>).MakeGenericType(serviceType));

        private bool ContainsExplicitRegistrationFor(Type serviceType) =>
            GetRegistrationEvenIfInvalid(serviceType, InjectionConsumerInfo.Root, false) != null;

        private void ThrowNotConstructableException(Type concreteType)
        {
            // At this point we know the concreteType is either NOT constructable or
            // Options.ResolveUnregisteredConcreteTypes is configured to not return the type.
            Options.IsConstructableType(concreteType, out string? exceptionMessage);

            throw new ActivationException(
                StringResources.ImplicitRegistrationCouldNotBeMadeForType(
                    this, concreteType, HasRegistrations)
                + " " + exceptionMessage);
        }
    }
}