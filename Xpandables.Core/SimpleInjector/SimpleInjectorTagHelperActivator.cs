﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Integration.AspNetCore.Mvc
{
    using System;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>Tag Helper Activator for Simple Injector.</summary>
    public class SimpleInjectorTagHelperActivator : ITagHelperActivator
    {
        private readonly Container container;
        private readonly Predicate<Type>? tagHelperSelector;
        private readonly ITagHelperActivator? frameworkTagHelperActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorTagHelperActivator"/> class.
        /// </summary>
        /// <param name="container">The container instance.</param>
        [Obsolete("Please use the other constructor overload or use the " +
            "SimpleInjectorAspNetCoreMvcIntegrationExtensions.AddSimpleInjectorTagHelperActivation " +
            "extension method instead. " +
            "Will be removed in version 5.0.",
            error: true)]
        public SimpleInjectorTagHelperActivator(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.container = container;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorTagHelperActivator"/> class.
        /// </summary>
        /// <param name="container">The container instance.</param>
        /// <param name="tagHelperSelector">The predicate that determines which tag helpers should be created
        /// by the supplied <paramref name="container"/> (when the predicate returns true) or using the
        /// supplied <paramref name="frameworkTagHelperActivator"/> (when the predicate returns false).</param>
        /// <param name="frameworkTagHelperActivator">The framework's tag helper activator.</param>
        public SimpleInjectorTagHelperActivator(
            Container container,
            Predicate<Type> tagHelperSelector,
            ITagHelperActivator frameworkTagHelperActivator)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (tagHelperSelector == null)
            {
                throw new ArgumentNullException(nameof(tagHelperSelector));
            }

            if (frameworkTagHelperActivator == null)
            {
                throw new ArgumentNullException(nameof(frameworkTagHelperActivator));
            }

            this.container = container;
            this.tagHelperSelector = tagHelperSelector;
            this.frameworkTagHelperActivator = frameworkTagHelperActivator;
        }

        /// <summary>Creates an <see cref="ITagHelper"/>.</summary>
        /// <typeparam name="TTagHelper">The <see cref="ITagHelper"/> type.</typeparam>
        /// <param name="context">The <see cref="ViewContext"/> for the executing view.</param>
        /// <returns>The tag helper.</returns>
        public TTagHelper Create<TTagHelper>(ViewContext context) where TTagHelper : ITagHelper =>
            this.tagHelperSelector?.Invoke(typeof(TTagHelper)) ?? true
                ? (TTagHelper)this.container.GetInstance(typeof(TTagHelper))
                : this.frameworkTagHelperActivator!.Create<TTagHelper>(context);
    }
}