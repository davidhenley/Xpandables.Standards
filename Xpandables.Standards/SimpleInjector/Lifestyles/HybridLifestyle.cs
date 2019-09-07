// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Lifestyles
{
    using System;

    internal sealed class HybridLifestyle : Lifestyle, IHybridLifestyle
    {
        private readonly Predicate<Container> lifestyleSelector;
        private readonly Lifestyle trueLifestyle;
        private readonly Lifestyle falseLifestyle;

        internal HybridLifestyle(
            Predicate<Container> lifestyleSelector, Lifestyle trueLifestyle, Lifestyle falseLifestyle)
            : base("Hybrid " + GetHybridName(trueLifestyle) + " / " + GetHybridName(falseLifestyle))
        {
            this.lifestyleSelector = lifestyleSelector;
            this.trueLifestyle = trueLifestyle;
            this.falseLifestyle = falseLifestyle;
        }

        public override int Length =>
            throw new NotSupportedException("The length property is not supported for this lifestyle.");

        string IHybridLifestyle.GetHybridName() =>
            GetHybridName(trueLifestyle) + " / " + GetHybridName(falseLifestyle);

        internal override int ComponentLength(Container container) =>
            Math.Max(
                trueLifestyle.ComponentLength(container),
                falseLifestyle.ComponentLength(container));

        internal override int DependencyLength(Container container) =>
            Math.Min(
                trueLifestyle.DependencyLength(container),
                falseLifestyle.DependencyLength(container));

        internal static string GetHybridName(Lifestyle lifestyle) =>
            (lifestyle as IHybridLifestyle)?.GetHybridName() ?? lifestyle.Name;

        protected internal override Registration CreateRegistrationCore<TConcrete>(Container container)
        {
            Func<bool> test = () => lifestyleSelector(container);

            return new HybridRegistration(
                typeof(TConcrete),
                test,
                trueLifestyle.CreateRegistration<TConcrete>(container),
                falseLifestyle.CreateRegistration<TConcrete>(container),
                this,
                container);
        }

        protected internal override Registration CreateRegistrationCore<TService>(
            Func<TService> instanceCreator, Container container)
        {
            Func<bool> test = () => lifestyleSelector(container);

            return new HybridRegistration(
                typeof(TService),
                test,
                trueLifestyle.CreateRegistration(instanceCreator, container),
                falseLifestyle.CreateRegistration(instanceCreator, container),
                this,
                container);
        }
    }
}