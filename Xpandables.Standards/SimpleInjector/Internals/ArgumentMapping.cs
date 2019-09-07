// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A map containing a generic argument (such as T) and the concrete type (such as Int32) that it
    /// represents.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    [SuppressMessage(
        "Design", "CA1067:Remplacer Object.Equals(object) au moment d'implémenter IEquatable<T>", Justification = "<En attente>")]
    internal sealed class ArgumentMapping : IEquatable<ArgumentMapping>
    {
        internal ArgumentMapping(Type argument, Type concreteType)
        {
            Argument = argument;
            ConcreteType = concreteType;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called by the debugger.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplay =>
            $"{nameof(Argument)}: {Argument.ToFriendlyName()}, " +
            $"{nameof(ConcreteType)}: {ConcreteType.ToFriendlyName()}";

        [DebuggerDisplay("{Argument, nq}")]
        internal Type Argument { get; }

        [DebuggerDisplay("{ConcreteType, nq}")]
        internal Type ConcreteType { get; }

        internal bool TypeConstraintsAreSatisfied => Validator.AreTypeConstraintsSatisfied();

        private TypeConstraintValidator Validator => new TypeConstraintValidator(this);

        /// <summary>Implements equality. Needed for doing LINQ distinct operations.</summary>
        /// <param name="other">The other to compare to.</param>
        /// <returns>True or false.</returns>
        bool IEquatable<ArgumentMapping>.Equals(ArgumentMapping other) =>
            Argument == other.Argument && ConcreteType == other.ConcreteType;

        /// <summary>Overrides the default hash code. Needed for doing LINQ distinct operations.</summary>
        /// <returns>An 32 bit integer.</returns>
        public override int GetHashCode() =>
            Argument.GetHashCode() ^ ConcreteType.GetHashCode();

        internal static ArgumentMapping Create(Type argument, Type concreteType) =>
            new ArgumentMapping(argument, concreteType);

        internal static ArgumentMapping[] Zip(Type[] arguments, Type[] concreteTypes) =>
            arguments.Zip(concreteTypes, Create).ToArray();

        internal bool ConcreteTypeMatchesPartialArgument()
        {
            if (Argument.IsGenericParameter || Argument == ConcreteType)
            {
                return true;
            }
            else if (!ConcreteType.IsGenericType || !Argument.IsGenericType)
            {
                return false;
            }
            else if (ConcreteType.GetGenericTypeDefinition() != Argument.GetGenericTypeDefinition())
            {
                return false;
            }
            else
            {
                return Argument.GetGenericArguments()
                    .Zip(ConcreteType.GetGenericArguments(), Create)
                    .All(mapping => mapping.ConcreteTypeMatchesPartialArgument());
            }
        }
    }
}