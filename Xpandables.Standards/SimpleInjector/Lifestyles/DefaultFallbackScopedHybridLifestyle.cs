// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Lifestyles
{
    using System;

    internal sealed class DefaultFallbackScopedHybridLifestyle : ScopedLifestyle, IHybridLifestyle
    {
        private readonly ScopedLifestyle defaultLifestyle;
        private readonly ScopedLifestyle fallbackLifestyle;

        internal DefaultFallbackScopedHybridLifestyle(
            ScopedLifestyle defaultLifestyle, ScopedLifestyle fallbackLifestyle)
            : base("Hybrid " + GetHybridName(defaultLifestyle) + " / " + GetHybridName(fallbackLifestyle))
        {
            this.defaultLifestyle = defaultLifestyle;
            this.fallbackLifestyle = fallbackLifestyle;
        }

        string IHybridLifestyle.GetHybridName() =>
            GetHybridName(defaultLifestyle) + " / " + GetHybridName(fallbackLifestyle);

        internal override int ComponentLength(Container container) =>
            Math.Max(
                defaultLifestyle.ComponentLength(container),
                fallbackLifestyle.ComponentLength(container));

        internal override int DependencyLength(Container container) =>
            Math.Min(
                defaultLifestyle.DependencyLength(container),
                fallbackLifestyle.DependencyLength(container));

        protected internal override Func<Scope?> CreateCurrentScopeProvider(Container container)
        {
            var defaultProvider = defaultLifestyle.CreateCurrentScopeProvider(container);
            var fallbackProvider = fallbackLifestyle.CreateCurrentScopeProvider(container);

            // NOTE: It is important to return a delegate that evaluates the lifestyleSelector on each call,
            // instead of evaluating the lifestyleSelector directly and returning either the trueProvider or
            // falseProvider. That behavior would be completely flawed, because that would burn the lifestyle
            // that is active during the compilation of the InstanceProducer's delegate right into that
            // delegate making the other lifestyle unavailable.
            return () => defaultProvider() ?? fallbackProvider();
        }

        protected override Scope? GetCurrentScopeCore(Container container) =>
            defaultLifestyle.GetCurrentScope(container)
                ?? fallbackLifestyle.GetCurrentScope(container);

        private static string GetHybridName(Lifestyle lifestyle) => HybridLifestyle.GetHybridName(lifestyle);
    }
}