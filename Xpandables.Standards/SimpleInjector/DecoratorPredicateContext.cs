﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using SimpleInjector.Advanced;
    using SimpleInjector.Decorators;

    /// <summary>
    /// An instance of this type will be supplied to the <see cref="Predicate{T}"/>
    /// delegate that is that is supplied to the
    /// <see cref="Container.RegisterDecorator(Type, Type, Predicate{DecoratorPredicateContext})">RegisterDecorator</see>
    /// overload that takes this delegate. This type contains information about the decoration that is about
    /// to be applied and it allows users to examine the given instance to see whether the decorator should
    /// be applied or not.
    /// </summary>
    /// <remarks>
    /// Please see the
    /// <see cref="Container.RegisterDecorator(Type, Type, Predicate{DecoratorPredicateContext})">RegisterDecorator</see>
    /// method for more information.
    /// </remarks>
    [DebuggerDisplay(nameof(DecoratorPredicateContext) +
        " ({" + nameof(DecoratorPredicateContext.DebuggerDisplay) + ", nq})")]
    public sealed class DecoratorPredicateContext : ApiObject
    {
        internal DecoratorPredicateContext(
            Type serviceType,
            Type implementationType,
            ReadOnlyCollection<Type> appliedDecorators,
            Expression expression)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            AppliedDecorators = appliedDecorators;
            Expression = expression;
        }

        /// <summary>
        /// Gets the closed generic service type for which the decorator is about to be applied. The original
        /// service type will be returned, even if other decorators have already been applied to this type.
        /// </summary>
        /// <value>The closed generic service type.</value>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the type of the implementation that is created by the container and for which the decorator
        /// is about to be applied. The original implementation type will be returned, even if other decorators
        /// have already been applied to this type. Please not that the implementation type can not always be
        /// determined. In that case the closed generic service type will be returned.
        /// </summary>
        /// <value>The implementation type.</value>
        public Type ImplementationType { get; }

        /// <summary>
        /// Gets the list of the types of decorators that have already been applied to this instance.
        /// </summary>
        /// <value>The applied decorators.</value>
        public ReadOnlyCollection<Type> AppliedDecorators { get; }

        /// <summary>
        /// Gets the current <see cref="Expression"/> object that describes the intention to create a new
        /// instance with its currently applied decorators.
        /// </summary>
        /// <value>The current expression that is about to be decorated.</value>
        public Expression Expression { get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called by the debugger.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplay => string.Format(
            CultureInfo.InvariantCulture,
            "{0} = {1}, {2} = {3}",
            nameof(ServiceType),
            ServiceType.ToFriendlyName(),
            nameof(ImplementationType),
            ImplementationType.ToFriendlyName());

        internal static DecoratorPredicateContext CreateFromInfo(
            Type serviceType, Expression expression, ServiceTypeDecoratorInfo info)
        {
            var appliedDecorators = new ReadOnlyCollection<Type>(
                info.AppliedDecorators.Select(d => d.DecoratorType).ToList());

            return new DecoratorPredicateContext(
                serviceType, info.ImplementationType, appliedDecorators, expression);
        }
    }
}