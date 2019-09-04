// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Lifestyles
{
    using System;

    internal sealed class ScopeManager
    {
        private readonly Container container;
        private readonly Func<Scope?> scopeRetriever;
        private readonly Action<Scope?> scopeReplacer;

        internal ScopeManager(Container container, Func<Scope?> scopeRetriever, Action<Scope?> scopeReplacer)
        {
            Requires.IsNotNull(container, nameof(container));
            Requires.IsNotNull(scopeRetriever, nameof(scopeRetriever));
            Requires.IsNotNull(scopeReplacer, nameof(scopeReplacer));

            this.container = container;
            this.scopeRetriever = scopeRetriever;
            this.scopeReplacer = scopeReplacer;
        }

        internal Scope? CurrentScope => GetCurrentScopeWithAutoCleanup();

        private Scope? CurrentScopeInternal
        {
            get { return scopeRetriever(); }
            set { scopeReplacer(value); }
        }

        internal Scope BeginScope() =>
            CurrentScopeInternal = new Scope(container, this, GetCurrentScopeWithAutoCleanup());

        internal void RemoveScope(Scope scope)
        {
            // If the scope is not the current scope or one of its ancestors, this means that either one of
            // the scope's parents have already been disposed, or the scope is disposed on a completely
            // unrelated thread. In both cases we shouldn't change the CurrentScope, since doing this,
            // since would cause an invalid scope to be registered as the current scope (this scope will
            // either be disposed or does not belong to the current execution context).
            if (IsScopeInLocalChain(scope))
            {
                CurrentScopeInternal = scope.ParentScope;
            }
        }

        // Determines whether this instance is the currently registered lifetime scope or an ancestor of it.
        private bool IsScopeInLocalChain(Scope scope)
        {
            Scope? localScope = CurrentScopeInternal;

            while (localScope != null)
            {
                if (object.ReferenceEquals(scope, localScope))
                {
                    return true;
                }

                localScope = localScope.ParentScope;
            }

            return false;
        }

        private Scope? GetCurrentScopeWithAutoCleanup()
        {
            Scope? scope = CurrentScopeInternal;

            // When the current scope is disposed, make the parent scope the current.
            while (scope != null && scope.Disposed)
            {
                CurrentScopeInternal = scope = scope.ParentScope;
            }

            return scope;
        }
    }
}