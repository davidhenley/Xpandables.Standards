// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Advanced
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed partial class PropertyInjectionHelper
    {
        private const int MaximumNumberOfFuncArguments = 16;
        private const int MaximumNumberOfPropertiesPerDelegate = MaximumNumberOfFuncArguments - 1;

        private static readonly ReadOnlyCollection<Type> FuncTypes = new ReadOnlyCollection<Type>(new Type[]
            {
                typeof(Func<>),
                typeof(Func<,>),
                typeof(Func<,,>),
                typeof(Func<,,,>),
                typeof(Func<,,,,>),
                typeof(Func<,,,,,>),
                typeof(Func<,,,,,,>),
                typeof(Func<,,,,,,,>),
                typeof(Func<,,,,,,,,>),
                typeof(Func<,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,,>),
            });

        private readonly Container container;
        private readonly Type implementationType;

        internal PropertyInjectionHelper(Container container, Type implementationType)
        {
            this.container = container;
            this.implementationType = implementationType;
        }

        internal static PropertyInjectionData BuildPropertyInjectionExpression(
            Container container,
            Type implementationType,
            PropertyInfo[] properties,
            Expression expressionToWrap)
        {
            var helper = new PropertyInjectionHelper(container, implementationType);

            return helper.BuildPropertyInjectionExpression(expressionToWrap, properties);
        }

        internal static PropertyInfo[] GetCandidateInjectionPropertiesFor(Type implementationType)
        {
            return implementationType.GetRuntimeProperties().ToArray();
        }

        internal static void VerifyProperties(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                VerifyProperty(property);
            }
        }

        private Delegate BuildPropertyInjectionDelegate(PropertyInfo[] properties)
        {
            try
            {
                return BuildPropertyInjectionDelegateInternal(properties);
            }
            catch (MemberAccessException ex)
            {
                // This happens when the user tries to resolve an internal type inside a (Silverlight) sandbox.
                throw new ActivationException(
                    StringResources.UnableToInjectPropertiesDueToSecurityConfiguration(implementationType,
                        ex),
                    ex);
            }
        }

        private Delegate BuildPropertyInjectionDelegateInternal(PropertyInfo[] properties)
        {
            var targetParameter = Expression.Parameter(implementationType, implementationType.Name);

            var dependencyParameters = (
                from property in properties
                select Expression.Parameter(property.PropertyType, property.Name))
                .ToArray();

            var propertyInjectionExpressions =
                BuildPropertyInjectionExpressions(targetParameter, properties, dependencyParameters);

            Type funcType = GetFuncType(properties, implementationType);

            var parameters = dependencyParameters.Concat(new[] { targetParameter });

            var lambda = Expression.Lambda(
                funcType,
                Expression.Block(implementationType, propertyInjectionExpressions),
                parameters);

            return CompilePropertyInjectorLambda(lambda);
        }

        private List<Expression> BuildPropertyInjectionExpressions(ParameterExpression targetParameter,
            PropertyInfo[] properties,
            ParameterExpression[] dependencyParameters)
        {
            var blockExpressions = (
                from pair in properties.Zip(dependencyParameters, (prop, param) => new { prop, param })
                select Expression.Assign(Expression.Property(targetParameter, pair.prop), pair.param))
                .Cast<Expression>()
                .ToList();

            var returnTarget = Expression.Label(implementationType);

            blockExpressions.Add(Expression.Return(returnTarget, targetParameter, implementationType));
            blockExpressions.Add(Expression.Label(returnTarget, Expression.Constant(null, implementationType)));

            return blockExpressions;
        }

        private static void VerifyProperty(PropertyInfo property)
        {
            MethodInfo? setMethod = property.GetSetMethod(nonPublic: true);

            if (setMethod == null)
            {
                throw new ActivationException(StringResources.PropertyHasNoSetter(property));
            }

            if (setMethod.IsStatic)
            {
                throw new ActivationException(StringResources.PropertyIsStatic(property));
            }
        }

        private PropertyInjectionData BuildPropertyInjectionExpression(
            Expression expression, PropertyInfo[] properties)
        {
            PropertyInjectionData data;

            // With MaximumNumberOfPropertiesPerDelegate = 4
            // new Impl { P1 = Dep1, P2 = Dep2, P3 = Dep3, P4 = Dep4, P5 = Dep5, P6 = Dep6, P6 = Dep7)
            // We build up an expression like this:
            // () => func1(Dep1, Dep2, Dep3, func2(Dep4, Dep5, Dep6, func3(Dep7, new Impl())))
            if (properties.Length > MaximumNumberOfPropertiesPerDelegate)
            {
                // Expression becomes: Func<Prop8, Prop9, ... , PropN, TargetType>
                var restProperties = properties.Skip(MaximumNumberOfPropertiesPerDelegate).ToArray();

                // Properties becomes { Prop1, Prop2, ..., Prop7 }.
                properties = properties.Take(MaximumNumberOfPropertiesPerDelegate).ToArray();

                data = BuildPropertyInjectionExpression(expression, restProperties);
            }
            else
            {
                data = new PropertyInjectionData(expression);
            }

            InstanceProducer[] producers = GetPropertyInstanceProducers(properties);

            var arguments = producers.Select(p => p.BuildExpression()).Concat(new[] { data.Expression });

            Delegate propertyInjectionDelegate = BuildPropertyInjectionDelegate(properties);

            return new PropertyInjectionData(
                expression: Expression.Invoke(Expression.Constant(propertyInjectionDelegate), arguments),
                producers: producers.Concat(data.Producers),
                properties: properties.Concat(data.Properties));
        }

        private InstanceProducer[] GetPropertyInstanceProducers(PropertyInfo[] properties)
        {
            return properties.Select(GetPropertyExpression).ToArray();
        }

        private InstanceProducer GetPropertyExpression(PropertyInfo property)
        {
            var consumer = new InjectionConsumerInfo(implementationType, property);

            return container.Options.GetInstanceProducerFor(consumer);
        }

        private static Type GetFuncType(PropertyInfo[] properties, Type injecteeType)
        {
            var genericTypeArguments = new List<Type>();

            genericTypeArguments.AddRange(from property in properties select property.PropertyType);

            genericTypeArguments.Add(injecteeType);

            // Return type is TResult. This is always the last generic type.
            genericTypeArguments.Add(injecteeType);

            int numberOfInputArguments = genericTypeArguments.Count;

            Type openGenericFuncType = FuncTypes[numberOfInputArguments - 1];

            return openGenericFuncType.MakeGenericType(genericTypeArguments.ToArray());
        }

        private Delegate CompilePropertyInjectorLambda(LambdaExpression expression)
        {
            Delegate? compiledDelegate = null;

            TryCompileLambdaInDynamicAssembly(expression, ref compiledDelegate);

            return compiledDelegate ?? expression.Compile();
        }

        partial void TryCompileLambdaInDynamicAssembly(LambdaExpression expression, ref Delegate? compiledDelegate);

        internal struct PropertyInjectionData
        {
            public readonly Expression Expression;
            public readonly IEnumerable<InstanceProducer> Producers;
            public readonly IEnumerable<PropertyInfo> Properties;

            public PropertyInjectionData(
                Expression expression,
                IEnumerable<InstanceProducer>? producers = null,
                IEnumerable<PropertyInfo>? properties = null)
            {
                Expression = expression;
                Producers = producers ?? Enumerable.Empty<InstanceProducer>();
                Properties = properties ?? Enumerable.Empty<PropertyInfo>();
            }
        }
    }
}