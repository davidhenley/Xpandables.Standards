﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Diagnostics
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Specifies the list of diagnostic types that are currently supported by the diagnostic
    /// <see cref="Analyzer"/>. Note that new diagnostic types might be added in future versions.
    /// For more information, please read the
    /// <a href="https://simpleinjector.org/diagnostics">Diagnosing your configuration using the Diagnostic
    /// Services</a> wiki documentation.
    /// </summary>
    public enum DiagnosticType
    {
        /// <summary>
        /// Diagnostic type that warns about
        /// a concrete type that was not registered explicitly and was not resolved using unregistered type
        /// resolution, but was created by the container using the transient lifestyle.
        /// For more information, see: https://simpleinjector.org/diaut.
        /// </summary>
        [Documentation("Container-Registered Component", "https://simpleinjector.org/diaut")]
        ContainerRegisteredComponent = 0,

        /// <summary>
        /// Diagnostic type that warns when a
        /// component depends on a service with a lifestyle that is shorter than that of the component.
        /// For more information, see: https://simpleinjector.org/dialm.
        /// </summary>
        [Documentation("Lifestyle Mismatch", "https://simpleinjector.org/dialm")]
        LifestyleMismatch = 1,

        /// <summary>
        /// Diagnostic type that warns when a
        /// component depends on an unregistered concrete type and this concrete type has a lifestyle that is
        /// different than the lifestyle of an explicitly registered type that uses this concrete type as its
        /// implementation.
        /// For more information, see: https://simpleinjector.org/diasc.
        /// </summary>
        [Documentation("Short Circuited Dependency", "https://simpleinjector.org/diasc")]
        ShortCircuitedDependency = 2,

        /// <summary>
        /// Diagnostic type that warns when a component depends on (too) many services.
        /// For more information, see: https://simpleinjector.org/diasr.
        /// </summary>
        [Documentation("SRP Violation", "https://simpleinjector.org/diasr")]
        SingleResponsibilityViolation = 3,

        /// <summary>
        /// Diagnostic type that warns when multiple registrations map to the same component and
        /// lifestyle, which might cause multiple instances to be created during the lifespan of that lifestyle.
        /// For more information, see: https://simpleinjector.org/diatl.
        /// </summary>
        [Documentation("Torn Lifestyle", "https://simpleinjector.org/diatl")]
        TornLifestyle = 4,

        /// <summary>
        /// Diagnostic type that warns when a component is registered as transient, while implementing
        /// <see cref="IDisposable"/>.
        /// For more information, see: https://simpleinjector.org/diadt.
        /// </summary>
        [Documentation("Disposable Transient Component", "https://simpleinjector.org/diadt")]
        DisposableTransientComponent = 5,

        /// <summary>
        /// Diagnostic type that warns when multiple registrations exist that map to the same component but
        /// with different lifestyles, which will cause the component to be cached in different -possibly
        /// incompatible- ways.
        /// For more information, see: https://simpleinjector.org/diaal.
        /// </summary>
        [Documentation("Ambiguous Lifestyles", "https://simpleinjector.org/diaal")]
        AmbiguousLifestyles = 6
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    internal sealed class DocumentationAttribute : Attribute
    {
        public readonly string Name;
        public readonly Uri DocumentationUrl;

        public DocumentationAttribute(string name, string documentationUrl)
        {
            Name = name;
            DocumentationUrl = new Uri(documentationUrl);
        }

        internal static DocumentationAttribute GetDocumentationAttribute(DiagnosticType value)
        {
            var members = typeof(DiagnosticType).GetMember(value.ToString());

            var attributes =
                from member in members
                from attribute in member.GetCustomAttributes(typeof(DocumentationAttribute), false)
                select (DocumentationAttribute)attribute;

            return attributes.FirstOrDefault() ?? new DocumentationAttribute(value.ToString(), string.Empty);
        }
    }
}