// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Advanced.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This is an internal type. Only depend on this type when you want to be absolutely sure a future
    /// version of the framework will break your code.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes",
        Justification = "This struct is not intended for public use.")]
    public struct LazyScope
    {
        private readonly Container container;
        private Func<Scope>? scopeFactory;
        private Scope? value;

        /// <summary>Initializes a new instance of the <see cref="LazyScope"/> struct.</summary>
        /// <param name="scopeFactory">The scope factory.</param>
        /// <param name="container">The container.</param>
        public LazyScope(Func<Scope> scopeFactory, Container container)
        {
            this.scopeFactory = scopeFactory;
            this.container = container;
            value = null;
        }

        /// <summary>Gets the lazily initialized Scope of the current LazyScope instance.</summary>
        /// <value>The current Scope or null.</value>
        public Scope Value
        {
            get
            {
                if (scopeFactory != null)
                {
                    value = container.GetVerificationOrResolveScopeForCurrentThread()
                        ?? scopeFactory.Invoke();
                    scopeFactory = null;
                }

                return value!;
            }
        }
    }
}