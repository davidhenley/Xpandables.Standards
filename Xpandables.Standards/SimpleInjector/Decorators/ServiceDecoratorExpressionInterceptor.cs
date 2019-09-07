// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SimpleInjector.Advanced;

    internal sealed class ServiceDecoratorExpressionInterceptor : DecoratorExpressionInterceptor
    {
        private readonly Dictionary<InstanceProducer, Registration> registrations;
        private readonly ExpressionBuiltEventArgs e;
        private readonly Type registeredServiceType;

        public ServiceDecoratorExpressionInterceptor(
            DecoratorExpressionInterceptorData data,
            Dictionary<InstanceProducer, Registration> registrations,
            ExpressionBuiltEventArgs e)
            : base(data)
        {
            this.registrations = registrations;
            this.e = e;
            registeredServiceType = e.RegisteredServiceType;
        }

        internal bool SatisfiesPredicate()
        {
            Context = CreatePredicateContext(e);

            return SatisfiesPredicate(Context);
        }

        internal void ApplyDecorator(Type closedDecoratorType)
        {
            var decoratorConstructor = Container.Options.SelectConstructor(closedDecoratorType);

            if (object.ReferenceEquals(Lifestyle, Container.SelectionBasedLifestyle))
            {
                Lifestyle = Container.Options.SelectLifestyle(closedDecoratorType);
            }

            // By creating the decorator using a Lifestyle Registration the decorator can be completely
            // incorporated into the pipeline. This means that the ExpressionBuilding can be applied,
            // properties can be injected, and it can be wrapped with an initializer.
            Registration decoratorRegistration = CreateRegistrationForDecorator(decoratorConstructor);

            ReplaceOriginalExpression(decoratorRegistration);

            AddAppliedDecoratorToPredicateContext(
                decoratorRegistration.GetRelationships(), decoratorConstructor);
        }

        private void ReplaceOriginalExpression(Registration decoratorRegistration)
        {
            e.Expression = decoratorRegistration.BuildExpression();

            e.ReplacedRegistration = decoratorRegistration;

            e.InstanceProducer.IsDecorated = true;

            // Must be called after calling BuildExpression, because otherwise we won't have any relationships
            MarkDecorateeFactoryRelationshipAsInstanceCreationDelegate(
                decoratorRegistration.GetRelationships());
        }

        private void MarkDecorateeFactoryRelationshipAsInstanceCreationDelegate(
            KnownRelationship[] relationships)
        {
            foreach (Registration dependency in GetDecorateeFactoryDependencies(relationships))
            {
                // Mark the dependency of the decoratee factory
                dependency.WrapsInstanceCreationDelegate = true;
            }
        }

        private IEnumerable<Registration> GetDecorateeFactoryDependencies(KnownRelationship[] relationships) =>
            from relationship in relationships
            where DecoratorHelpers.IsScopelessDecorateeFactoryDependencyType(
                relationship.Dependency.ServiceType, e.RegisteredServiceType)
            select relationship.Dependency.Registration;

        private Registration CreateRegistrationForDecorator(ConstructorInfo decoratorConstructor)
        {
            Registration registration;

            // Ensure that the registration for the decorator is created only once to prevent the possibility
            // of multiple instances being created when dealing lifestyles that cache an instance within the
            // Registration instance itself (such as the Singleton lifestyle does).
            lock (registrations)
            {
                if (!registrations.TryGetValue(e.InstanceProducer, out registration))
                {
                    registration = CreateRegistration(
                        registeredServiceType,
                        decoratorConstructor,
                        e.Expression,
                        e.InstanceProducer,
                        GetServiceTypeInfo(e));

                    registrations[e.InstanceProducer] = registration;
                }
            }

            return registration;
        }

        private void AddAppliedDecoratorToPredicateContext(
            IEnumerable<KnownRelationship> decoratorRelationships, ConstructorInfo decoratorConstructor)
        {
            var info = GetServiceTypeInfo(e);

            // Add the decorator to the list of applied decorators. This way users can use this information in
            // the predicate of the next decorator they add.
            info.AddAppliedDecorator(
                e.RegisteredServiceType,
                decoratorConstructor.DeclaringType,
                Container,
                Lifestyle,
                e.Expression,
                decoratorRelationships);
        }
    }
}