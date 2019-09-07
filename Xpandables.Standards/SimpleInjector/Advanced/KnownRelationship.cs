﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Advanced
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// A known relationship defines a relationship between two types. The Diagnostics Debug View uses this
    /// information to spot possible misconfigurations.
    /// </summary>
    [DebuggerDisplay("{" + nameof(KnownRelationship.DebuggerDisplay) + ",nq}")]
    public sealed class KnownRelationship : IEquatable<KnownRelationship?>
    {
        // This constructor is here for backwards compatibility: the library itself uses the internal ctor.

        /// <summary>Initializes a new instance of the <see cref="KnownRelationship"/> class.</summary>
        /// <param name="implementationType">The implementation type of the parent type.</param>
        /// <param name="lifestyle">The lifestyle of the parent type.</param>
        /// <param name="dependency">The type that the parent depends on (it is injected into the parent).</param>
        public KnownRelationship(
            Type implementationType, Lifestyle lifestyle, InstanceProducer dependency)
        {
            Requires.IsNotNull(implementationType, nameof(implementationType));
            Requires.IsNotNull(lifestyle, nameof(lifestyle));
            Requires.IsNotNull(dependency, nameof(dependency));

            ImplementationType = implementationType;
            Lifestyle = lifestyle;
            Dependency = dependency;
            Consumer = InjectionConsumerInfo.Root;
        }

        internal KnownRelationship(
            Type implementationType,
            Lifestyle lifestyle,
            InjectionConsumerInfo consumer,
            InstanceProducer dependency,
            string? additionalInformation = null)
        {
            Requires.IsNotNull(implementationType, nameof(implementationType));
            Requires.IsNotNull(lifestyle, nameof(lifestyle));
            Requires.IsNotNull(consumer, nameof(consumer));
            Requires.IsNotNull(dependency, nameof(dependency));

            ImplementationType = implementationType;
            Lifestyle = lifestyle;
            Consumer = consumer;
            Dependency = dependency;
            AdditionalInformation = additionalInformation ?? string.Empty;
        }

        /// <summary>Gets the implementation type of the parent type of the relationship.</summary>
        /// <value>The implementation type of the parent type of the relationship.</value>
        [DebuggerDisplay("{" + nameof(ImplementationTypeDebuggerDisplay) + ", nq}")]
        public Type ImplementationType { get; }

        /// <summary>Gets the lifestyle of the parent type of the relationship.</summary>
        /// <value>The lifestyle of the parent type of the relationship.</value>
        public Lifestyle Lifestyle { get; }

        /// <summary>Gets the type that the parent depends on (it is injected into the parent).</summary>
        /// <value>The type that the parent depends on.</value>
        public InstanceProducer Dependency { get; }

        internal InjectionConsumerInfo Consumer { get; }

        internal string AdditionalInformation { get; } = string.Empty;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called by the debugger.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => string.Format(
            CultureInfo.InvariantCulture,
            "{0} = {1}, {2} = {3}, {4} = {{{5}}}",
            nameof(ImplementationType),
            ImplementationTypeDebuggerDisplay,
            nameof(Lifestyle),
            Lifestyle.Name,
            nameof(Dependency),
            Dependency.DebuggerDisplay);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called by the debugger.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string ImplementationTypeDebuggerDisplay => ImplementationType.ToFriendlyName();

        /// <inheritdoc />
        public override int GetHashCode() =>
            ImplementationType.GetHashCode()
            ^ Lifestyle.GetHashCode()
            ^ Consumer.GetHashCode()
            ^ Dependency.GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as KnownRelationship);

        /// <inheritdoc />
        public bool Equals(KnownRelationship? other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return
                ImplementationType.Equals(other.ImplementationType)
                && Lifestyle.Equals(other.Lifestyle)
                && Dependency.Equals(other.Dependency)
                && Consumer.Equals(other.Consumer);
        }
    }
}