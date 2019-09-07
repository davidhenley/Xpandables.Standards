// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Decorators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using SimpleInjector.Internals;
    using SimpleInjector.Lifestyles;

    // This class allows decorating collections of services with elements that are created out of the control
    // of the container. Collections are registered using the following methods:
    // -Collections.Register<TService>(IEnumerable<TService> uncontrolledCollection)
    // -Register<TService>(TService) (where TService is a IEnumerable<T>)
    // -Collections.Register(Type serviceType, IEnumerable uncontrolledCollection).
    internal sealed class ContainerUncontrolledServicesDecoratorInterceptor : DecoratorExpressionInterceptor
    {
        private readonly Dictionary<InstanceProducer, IEnumerable> singletonDecoratedCollectionsCache;
        private readonly ExpressionBuiltEventArgs e;
        private readonly Type registeredServiceType;

        private ConstructorInfo? decoratorConstructor;
        private Type? decoratorType;

        public ContainerUncontrolledServicesDecoratorInterceptor(
            DecoratorExpressionInterceptorData data,
            Dictionary<InstanceProducer, IEnumerable> singletonDecoratedCollectionsCache,
            ExpressionBuiltEventArgs e,
            Type registeredServiceType)
            : base(data)
        {
            this.singletonDecoratedCollectionsCache = singletonDecoratedCollectionsCache;
            this.e = e;
            this.registeredServiceType = registeredServiceType;
        }

        internal bool SatisfiesPredicate()
        {
            // We don't have an expression at this point, since the instances are not created by the container.
            // Therefore we fake an expression so it can still be passed on to the predicate the user might
            // have defined.
            var expression = Expression.Constant(null, registeredServiceType);

            var registration = new ExpressionRegistration(
                expression, registeredServiceType, Lifestyle.Unknown, Container);

            registration.ReplaceRelationships(e.InstanceProducer.GetRelationships());

            var info = GetServiceTypeInfo(
                e,
                originalExpression: expression,
                originalRegistration: registration,
                registeredServiceType: registeredServiceType);

            Context = CreatePredicateContext(registeredServiceType, expression, info);

            return SatisfiesPredicate(Context);
        }

        internal void SetDecorator(Type decorator)
        {
            decoratorConstructor = Container.Options.SelectConstructor(decorator);

            if (object.ReferenceEquals(Lifestyle, Container.SelectionBasedLifestyle))
            {
                Lifestyle = Container.Options.SelectLifestyle(decorator);
            }

            // The actual decorator could be different. TODO: must... write... test... for... this.
            decoratorType = decoratorConstructor.DeclaringType;
        }

        internal void ApplyDecorator()
        {
            var registration = new ExpressionRegistration(
                e.Expression, registeredServiceType, Lifestyle.Unknown, Container);

            registration.ReplaceRelationships(e.InstanceProducer.GetRelationships());

            var serviceTypeInfo = GetServiceTypeInfo(
                e,
                originalRegistration: registration,
                registeredServiceType: registeredServiceType);

            var decoratedExpression = BuildDecoratorExpression(out Registration decoratorRegistration);

            e.Expression = decoratedExpression;

            // Add the decorator to the list of applied decorator. This way users can use this
            // information in the predicate of the next decorator they add.
            serviceTypeInfo.AddAppliedDecorator(
                registeredServiceType,
                decoratorType!,
                Container,
                Lifestyle,
                decoratedExpression);

            e.KnownRelationships.AddRange(decoratorRegistration.GetRelationships());
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
            Justification = "This is not a performance critical path.")]
        private Expression BuildDecoratorExpression(out Registration decoratorRegistration)
        {
            Type decoratorTypeDefinition = DecoratorTypeDefinition!;

            ThrowWhenDecoratorNeedsAFunc(decoratorTypeDefinition);
            ThrownWhenLifestyleIsNotSupported(decoratorTypeDefinition);

            ParameterExpression parameter = Expression.Parameter(registeredServiceType, "decoratee");

            decoratorRegistration = CreateRegistrationForUncontrolledCollection(parameter);

            Expression parameterizedDecoratorExpression = decoratorRegistration.BuildExpression();

            // TODO: Optimize for performance by using a dynamic assembly where possible.
            Delegate wrapInstanceWithDecorator =
                BuildDecoratorWrapper(parameter, parameterizedDecoratorExpression)
                .Compile();

            Expression originalEnumerableExpression = e.Expression;

            if (originalEnumerableExpression is ConstantExpression)
            {
                var collection = ((ConstantExpression)originalEnumerableExpression).Value as IEnumerable;

                return BuildDecoratorEnumerableExpressionForConstantEnumerable(wrapInstanceWithDecorator,
                    collection!);
            }
            else
            {
                return BuildDecoratorEnumerableExpressionForNonConstantExpression(
                    wrapInstanceWithDecorator, originalEnumerableExpression);
            }
        }

        private Registration CreateRegistrationForUncontrolledCollection(Expression decorateeExpression)
        {
            var overriddenParameters = CreateOverriddenParameters(decorateeExpression);

            // Create the decorator as transient. Caching is applied later on.
            return Lifestyle.Transient.CreateDecoratorRegistration(
                decoratorConstructor!.DeclaringType, Container, overriddenParameters);
        }

        private OverriddenParameter[] CreateOverriddenParameters(Expression decorateeExpression)
        {
            ParameterInfo decorateeParameter =
                GetDecorateeParameter(registeredServiceType, decoratorConstructor!);

            decorateeExpression =
                GetExpressionForDecorateeDependencyParameterOrNull(
                    decorateeParameter, registeredServiceType, decorateeExpression)!;

            var currentProducer = GetServiceTypeInfo(e).GetCurrentInstanceProducer();

            var decorateeOverriddenParameter =
                new OverriddenParameter(decorateeParameter, decorateeExpression, currentProducer);

            IEnumerable<OverriddenParameter> predicateContextOverriddenParameters =
                CreateOverriddenDecoratorContextParameters(currentProducer);

            var overriddenParameters = (new[] { decorateeOverriddenParameter })
                .Concat(predicateContextOverriddenParameters);

            return overriddenParameters.ToArray();
        }

        private IEnumerable<OverriddenParameter> CreateOverriddenDecoratorContextParameters(
            InstanceProducer currentProducer)
        {
            return
                from parameter in decoratorConstructor!.GetParameters()
                where parameter.ParameterType == typeof(DecoratorContext)
                let contextExpression = Expression.Constant(new DecoratorContext(Context!))
                select new OverriddenParameter(parameter, contextExpression, currentProducer);
        }

        // Creates an expression that calls a Func<T, T> delegate that takes in the service and returns
        // that instance, wrapped with the decorator.
        private LambdaExpression BuildDecoratorWrapper(ParameterExpression parameter,
            Expression decoratorExpression)
        {
            Type funcType =
                typeof(Func<,>).MakeGenericType(registeredServiceType, registeredServiceType);

            return Expression.Lambda(funcType, decoratorExpression, parameter);
        }

        private Expression BuildDecoratorEnumerableExpressionForConstantEnumerable(
            Delegate wrapInstanceWithDecoratorDelegate, IEnumerable collection)
        {
            // Build the query: from item in collection select wrapInstanceWithDecorator(item);
            IEnumerable decoratedCollection =
                collection.Select(registeredServiceType, wrapInstanceWithDecoratorDelegate);

            // Passing the enumerable type is needed when running in the Silverlight sandbox.
            Type enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(registeredServiceType);

            if (Lifestyle == Lifestyle.Singleton)
            {
                Func<IEnumerable> collectionCreator = () =>
                {
                    Array array = ToArray(registeredServiceType, decoratedCollection);
                    return DecoratorHelpers.MakeReadOnly(registeredServiceType, array);
                };

                IEnumerable singleton = GetSingletonDecoratedCollection(collectionCreator);

                return Expression.Constant(singleton, enumerableServiceType);
            }

            return Expression.Constant(decoratedCollection, enumerableServiceType);
        }

        private Expression BuildDecoratorEnumerableExpressionForNonConstantExpression(
            Delegate wrapInstanceWithDecorator, Expression expression)
        {
            // Build the query: from item in expression select wrapInstanceWithDecorator(item);
            var callExpression =
                DecoratorHelpers.Select(expression, registeredServiceType, wrapInstanceWithDecorator);

            if (Lifestyle == Lifestyle.Singleton)
            {
                Type enumerableServiceType =
                    typeof(IEnumerable<>).MakeGenericType(registeredServiceType);

                Func<IEnumerable> collectionCreator = () =>
                {
                    Type funcType = typeof(Func<>).MakeGenericType(enumerableServiceType);
                    Delegate lambda = Expression.Lambda(funcType, callExpression).Compile();
                    var decoratedCollection = (IEnumerable)lambda.DynamicInvoke();
                    Array array = ToArray(registeredServiceType, decoratedCollection);
                    return DecoratorHelpers.MakeReadOnly(registeredServiceType, array);
                };

                IEnumerable singleton = GetSingletonDecoratedCollection(collectionCreator);

                // Passing the enumerable type is needed when running in a (Silverlight) sandbox.
                return Expression.Constant(singleton, enumerableServiceType);
            }

            return callExpression;
        }

        private void ThrowWhenDecoratorNeedsAFunc(Type decoratorTypeDefinition)
        {
            Type decorateeFactoryType = GetDecorateeFactoryTypeOrNull();

            if (decorateeFactoryType != null)
            {
                // decoratorType is never null at this point
                string message = StringResources.CantGenerateFuncForDecorator(
                    registeredServiceType,
                    decorateeFactoryType,
                    decoratorTypeDefinition ?? decoratorType!);

                throw new ActivationException(message);
            }
        }

        private Type GetDecorateeFactoryTypeOrNull() => (
            from parameter in decoratorConstructor!.GetParameters()
            where DecoratorHelpers.IsScopelessDecorateeFactoryDependencyType(
                parameter.ParameterType, registeredServiceType)
                || DecoratorHelpers.IsScopeDecorateeFactoryDependencyParameter(
                    parameter.ParameterType, registeredServiceType)
            select parameter.ParameterType)
            .FirstOrDefault();

        private void ThrownWhenLifestyleIsNotSupported(Type decoratorTypeDefinition)
        {
            // Because the user registered an IEnumerable<TService>, this collection can be dynamic in nature,
            // and the number of elements could change on each enumeration. It's impossible to detect if a
            // returned element is supposed to be a new element and should get its own new decorator, or if
            // it is supposed to be an existing element, for which an already cached decorator can be used.
            // In fact we can't really cache elements as Singleton, but since this was already supported in
            // the past, we don't want to introduce (yet another) breaking change.
            if (Lifestyle != Lifestyle.Transient && Lifestyle != Lifestyle.Singleton)
            {
                // At this point, decoratorType is never null.
                throw new NotSupportedException(
                    StringResources.CanNotDecorateContainerUncontrolledCollectionWithThisLifestyle(
                        decoratorTypeDefinition ?? decoratorType!,
                        Lifestyle,
                        registeredServiceType));
            }
        }

        private IEnumerable GetSingletonDecoratedCollection(Func<IEnumerable> collectionCreator)
        {
            lock (singletonDecoratedCollectionsCache)
            {
                IEnumerable collection;

                if (!singletonDecoratedCollectionsCache.TryGetValue(
                    e.InstanceProducer, out collection))
                {
                    collection = collectionCreator();

                    singletonDecoratedCollectionsCache[e.InstanceProducer] = collection;
                }

                return collection;
            }
        }

        private static Array ToArray(Type elementType, IEnumerable source)
        {
            object[] collection = source.Cast<object>().ToArray();
            Array array = Array.CreateInstance(elementType, collection.Length);
            Array.Copy(collection, array, collection.Length);

            return array;
        }
    }
}