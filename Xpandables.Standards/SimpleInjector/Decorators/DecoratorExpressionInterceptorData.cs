﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Decorators
{
    using System;

    internal sealed class DecoratorExpressionInterceptorData
    {
        public DecoratorExpressionInterceptorData(
            Container container,
            Type serviceType,
            Type? decoratorType,
            Predicate<DecoratorPredicateContext>? predicate,
            Lifestyle lifestyle,
            Func<DecoratorPredicateContext, Type>? factory = null)
        {
            Container = container;
            ServiceType = serviceType;
            DecoratorType = decoratorType;
            DecoratorTypeFactory = factory == null ? null : WrapInNullProtector(factory);
            Predicate = predicate;
            Lifestyle = lifestyle;
        }

        internal Container Container { get; }

        internal Type ServiceType { get; }

        internal Type? DecoratorType { get; }

        internal Func<DecoratorPredicateContext, Type>? DecoratorTypeFactory { get; }

        internal Predicate<DecoratorPredicateContext>? Predicate { get; }

        internal Lifestyle Lifestyle { get; }

        private Func<DecoratorPredicateContext, Type> WrapInNullProtector(
            Func<DecoratorPredicateContext, Type> decoratorTypeFactory)
        {
            return context =>
            {
                Type type = decoratorTypeFactory(context);

                if (type == null)
                {
                    throw new InvalidOperationException(
                        StringResources.DecoratorFactoryReturnedNull(ServiceType));
                }

                return type;
            };
        }
    }
}