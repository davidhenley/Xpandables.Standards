// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Allows validating an ArgumentMapping.
    /// </summary>
    internal sealed class TypeConstraintValidator
    {
        public TypeConstraintValidator(ArgumentMapping mapping)
        {
            Mapping = mapping;
        }

        internal ArgumentMapping Mapping { get; }

        internal bool AreTypeConstraintsSatisfied() =>
            ParameterSatisfiesNotNullableValueTypeConstraint()
            && ParameterSatisfiesDefaultConstructorConstraint()
            && ParameterSatisfiesReferenceTypeConstraint()
            && ParameterSatisfiesGenericParameterConstraints();

        private bool ParameterSatisfiesDefaultConstructorConstraint()
        {
            if (!MappingArgumentHasConstraint(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                return true;
            }

            if (Mapping.ConcreteType.IsGenericParameter)
            {
                // In case the concrete type itself is a generic parameter, it as well should have the "new()"
                // constraint. If not, it means that the "new()" constraint is added on the implementation.
                return MappingConcreteTypeHasConstraint(GenericParameterAttributes.DefaultConstructorConstraint);
            }

            if (Mapping.ConcreteType.IsValueType)
            {
                // Value types always have a default constructor.
                return true;
            }

            return HasDefaultConstructor(Mapping.ConcreteType);
        }

        private static bool HasDefaultConstructor(Type t) => t.GetConstructor(Helpers.Array<Type>.Empty) != null;

        private bool ParameterSatisfiesReferenceTypeConstraint()
        {
            if (!MappingArgumentHasConstraint(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                return true;
            }

            return !Mapping.ConcreteType.IsValueType;
        }

        private bool ParameterSatisfiesNotNullableValueTypeConstraint()
        {
            if (!MappingArgumentHasConstraint(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                return true;
            }

            if (!Mapping.ConcreteType.IsValueType)
            {
                return false;
            }

            bool isNullable = typeof(Nullable<>).IsGenericTypeDefinitionOf(Mapping.ConcreteType);

            return !isNullable;
        }

        private bool ParameterSatisfiesGenericParameterConstraints()
        {
            if (!Mapping.Argument.IsGenericParameter)
            {
                return true;
            }

            var unsatisfiedConstraints =
                from constraint in Mapping.Argument.GetGenericParameterConstraints()
                where !MappingMightBeCompatibleWithTypeConstraint(constraint)
                select constraint;

            return !unsatisfiedConstraints.Any();
        }

        private bool MappingMightBeCompatibleWithTypeConstraint(Type constraint)
        {
            // We return true in PCL, because there's no System.Type.GUID in PCL and GUID is needed to compare
            // if the constraint matches one of the base type. Returning true does not change the functional
            // behavior and correctness of the framework, but does lower the performance when resolving a
            // service for the first time. This can especially add up when calling Verify. In a benchmark of
            // a complex application that made heavily use of generic registrations with type constraints,
            // we've seen a call to Verify() take up to 6 times as long (from 8.5 seconds to 55 seconds), when
            // we don't do these checks here (and simply return true).
            // That's why we need to have these checks in the full version.
            if (constraint.IsAssignableFrom(Mapping.ConcreteType))
            {
                return true;
            }

            if (constraint.ContainsGenericParameters)
            {
                // The constraint is one of the other generic parameters, but this class checks a single
                // mapping, so we cannot check whether this constraint holds. We just return true and
                // have to check later on whether this constraint holds.
                return true;
            }

            var baseTypes = Mapping.ConcreteType.GetBaseTypesAndInterfaces();

            // This doesn't feel right, but have no idea how to reliably do this check without the GUID.
            return baseTypes.Any(type => type.GetGuid() == constraint.GetGuid());
        }

        private bool MappingArgumentHasConstraint(GenericParameterAttributes constraint) =>
            GenericParameterHasConstraint(Mapping.Argument, constraint);

        private bool MappingConcreteTypeHasConstraint(GenericParameterAttributes constraint) =>
            GenericParameterHasConstraint(Mapping.ConcreteType, constraint);

        private static bool GenericParameterHasConstraint(
            Type genericParameter, GenericParameterAttributes constraint)
        {
            if (!genericParameter.IsGenericParameter)
            {
                return false;
            }

            var constraints = genericParameter.GetGenericParameterAttributes();
            return (constraints & constraint) != GenericParameterAttributes.None;
        }
    }
}