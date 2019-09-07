﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Lifestyles
{
    using System;
    using System.Threading;

    /// <summary>
    /// Defines a lifestyle that caches instances during the lifetime of an explicitly defined scope using the
    /// <see cref="BeginScope(Container)">BeginScope</see>
    /// method. An execution context scope flows with the logical execution context. Scopes can be nested and
    /// nested scopes will get their own instance. Instances created by this lifestyle can be disposed when
    /// the created scope gets disposed.
    /// </summary>
    /// <example>
    /// The following example shows the usage of the <b>ExecutionContextScopeLifestyle</b> class:
    /// <code lang="cs"><![CDATA[
    /// var container = new Container();
    /// container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    /// container.Register<IUnitOfWork, EntityFrameworkUnitOfWork>(Lifestyle.Scoped);
    ///
    /// using (AsyncScopedLifestyle.BeginScope(container))
    /// {
    ///     var instance1 = container.GetInstance<IUnitOfWork>();
    ///     // ...
    /// }
    /// ]]></code>
    /// </example>
    public class AsyncScopedLifestyle : ScopedLifestyle
    {
        private static readonly object managerKey = new object();

        /// <summary>Initializes a new instance of the <see cref="AsyncScopedLifestyle"/> class.
        /// The created and cached instance will be disposed when the created
        /// <see cref="Scope"/> instance gets disposed and when the created object implements
        /// <see cref="IDisposable"/>.
        /// </summary>
        public AsyncScopedLifestyle() : base("Async Scoped")
        {
        }

        /// <summary>
        /// Begins a new scope for the given <paramref name="container"/>.
        /// Services, registered using the <see cref="AsyncScopedLifestyle"/> are cached during the
        /// lifetime of that scope. The scope should be disposed explicitly.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>A new <see cref="Scope"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="container"/> is a null reference.</exception>
        /// <example>
        /// <code lang="cs"><![CDATA[
        /// using (AsyncScopedLifestyle.BeginScope())
        /// {
        ///     var handler = (IRequestHandler)container.GetInstance(handlerType);
        ///
        ///     handler.Handle(request);
        /// }
        /// ]]></code>
        /// </example>
        public static Scope BeginScope(Container container)
        {
            Requires.IsNotNull(container, nameof(container));

            return GetScopeManager(container).BeginScope();
        }

        /// <summary>
        /// Returns the current <see cref="Scope"/> for this lifestyle and the given
        /// <paramref name="container"/>, or null when this method is executed outside the context of a scope.
        /// </summary>
        /// <param name="container">The container instance that is related to the scope to return.</param>
        /// <returns>A <see cref="Scope"/> instance or null when there is no scope active in this context.</returns>
        protected override Scope? GetCurrentScopeCore(Container container) =>
            GetScopeManager(container).CurrentScope;

        /// <summary>
        /// Creates a delegate that upon invocation return the current <see cref="Scope"/> for this
        /// lifestyle and the given <paramref name="container"/>, or null when the delegate is executed outside
        /// the context of such scope.
        /// </summary>
        /// <param name="container">The container for which the delegate gets created.</param>
        /// <returns>A <see cref="Func{T}"/> delegate. This method never returns null.</returns>
        protected internal override Func<Scope?> CreateCurrentScopeProvider(Container container)
        {
            ScopeManager manager = GetScopeManager(container);

            return () => manager.CurrentScope;
        }

        private static ScopeManager GetScopeManager(Container c) => c.ContainerScope.GetOrSetItem(managerKey, CreateManager);

        private static ScopeManager CreateManager(Container container, object key)
        {
            var asyncLocal = new AsyncLocal<Scope?>();

            return new ScopeManager(container, () => asyncLocal.Value, s => asyncLocal.Value = s);
        }
    }
}