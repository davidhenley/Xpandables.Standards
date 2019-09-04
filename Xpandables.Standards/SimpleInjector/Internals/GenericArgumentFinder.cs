﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Allows retrieving the concrete types of the generic type arguments of that must be used to create a
    /// closed generic implementation of a given open generic implementation, based on on the concrete
    /// arguments of the given closed base type.
    /// </summary>
    internal sealed class GenericArgumentFinder
    {
        private readonly IList<Type> serviceTypeDefinitionArguments;
        private readonly Type[] serviceTypeToResolveArguments;
        private readonly IList<Type> implementationTypeDefinitionArguments;
        private readonly Type[] partialImplementationArguments;

        public GenericArgumentFinder(
            Type serviceTypeDefinition,
            Type serviceTypeToResolve,
            Type implementationTypeDefinition,
            Type? partialOpenGenericImplementation)
        {
            serviceTypeDefinitionArguments = serviceTypeDefinition.GetGenericArguments();
            serviceTypeToResolveArguments = serviceTypeToResolve.GetGenericArguments();
            implementationTypeDefinitionArguments = implementationTypeDefinition.GetGenericArguments();
            partialImplementationArguments =
                (partialOpenGenericImplementation ?? implementationTypeDefinition).GetGenericArguments();
        }

        internal Type[] GetConcreteTypeArgumentsForClosedImplementation()
        {
            // The arguments must be in the same order as those of the open implementation.
            return (
                from mapping in FindArgumentMappings()
                orderby implementationTypeDefinitionArguments.IndexOf(mapping.Argument)
                select mapping.ConcreteType)
                .ToArray();
        }

        private ArgumentMapping[] FindArgumentMappings()
        {
            // An Argument mapping is a mapping between a generic type argument and a concrete type. For
            // instance: { Argument = T, ConcreteType = Int32 } is the mapping from generic type argument T
            // to Int32.
            var argumentMappings = GetOpenServiceArgumentToConcreteTypeMappings();

            ConvertToOpenImplementationArgumentMappings(ref argumentMappings);

            RemoveMappingsThatDoNotSatisfyAllTypeConstraints(ref argumentMappings);

            return argumentMappings.ToArray();
        }

        private IEnumerable<ArgumentMapping> GetOpenServiceArgumentToConcreteTypeMappings()
        {
            // Here we 'zip' the generic types (T, TKey, TValue) together with their concrete counter parts.
            var mappings = serviceTypeDefinitionArguments
                .Zip(serviceTypeToResolveArguments, ArgumentMapping.Create);

            var filledInPartials = implementationTypeDefinitionArguments
                .Zip(partialImplementationArguments, ArgumentMapping.Create)
                .Where(mapping => !mapping.ConcreteType.ContainsGenericParameter());

            return mappings.Concat(filledInPartials).ToArray();
        }

        private void ConvertToOpenImplementationArgumentMappings(ref IEnumerable<ArgumentMapping> mappings)
        {
            mappings = (
                from mapping in mappings
                from newMapping in ConvertToOpenImplementationArgumentMappings(mapping, new List<Type>())
                select newMapping)
                .ToArray();

            mappings = mappings.Distinct().ToArray();
        }

        private void RemoveMappingsThatDoNotSatisfyAllTypeConstraints(
            ref IEnumerable<ArgumentMapping> mappings)
        {
            mappings = (
                from mapping in mappings
                where mapping.TypeConstraintsAreSatisfied
                where CanBeMappedToSuppliedConcreteTypes(mapping)
                select mapping)
                .ToArray();
        }

        private bool CanBeMappedToSuppliedConcreteTypes(ArgumentMapping mapping)
        {
            var index = serviceTypeDefinitionArguments.IndexOf(mapping.Argument);

            // In case the mapping's concrete type is a generic parameter, it means this method is called
            // using an open-generic type and we can't verify whether a mapping is possible; we will return
            // true. Same holds when we can't find the mapping in the service type definition; we will assume
            // it can be mapped. This will be caught higher up the stack.
            return index < 0 || mapping.ConcreteType.IsGenericParameter
                ? true
                : mapping.ConcreteType == serviceTypeToResolveArguments[index];
        }

        private IEnumerable<ArgumentMapping> ConvertToOpenImplementationArgumentMappings(
            ArgumentMapping mapping, IList<Type> processedTypes)
        {
            // We are only interested in generic parameters
            if (mapping.Argument.IsGenericArgument() && !processedTypes.Contains(mapping.Argument))
            {
                processedTypes.Add(mapping.Argument);

                if (implementationTypeDefinitionArguments.Contains(mapping.Argument))
                {
                    // The argument is one of the type's generic arguments. We can directly return it.
                    yield return mapping;

                    foreach (var arg in GetTypeConstraintArgumentMappingsRecursive(mapping, processedTypes))
                    {
                        yield return arg;
                    }
                }
                else
                {
                    // The argument is not in the type's list, which means that the real type is (or are)
                    // buried in a generic type (i.e. Nullable<KeyValueType<TKey, TValue>>). This can result
                    // in multiple values.
                    foreach (var arg in ConvertToOpenImplementationArgumentMappingsRecursive(mapping, processedTypes))
                    {
                        yield return arg;
                    }
                }
            }
        }

        private ArgumentMapping[] GetTypeConstraintArgumentMappingsRecursive(ArgumentMapping mapping, IList<Type> processedTypes)
        {
            IEnumerable<Type> constraints = Enumerable.Empty<Type>();

            if (mapping.Argument.IsGenericParameter)
            {
                //// If the type itself is a generic parameter such as TKey (and not for instance IBar<TValue>)
                //// We must skip it, since there is no mappings we can extract from it (while IBar<TValue> could).
                constraints =
                    from constraint in mapping.Argument.GetGenericParameterConstraints()
                    where !constraint.IsGenericParameter
                    select constraint;
            }
            else
            {
                // In case we're dealing with partial generic type's (such as Lazy<List<T>>) the argument is
                // not a generic parameter and the argument (List<T> for instance) itself becomes the
                // constraints.
            }

            return (
                from constraint in constraints
                let constraintMapping = new ArgumentMapping(constraint, mapping.ConcreteType)
                from arg in ConvertToOpenImplementationArgumentMappings(constraintMapping, processedTypes).ToArray()
                select arg)
                .ToArray();
        }

        private ArgumentMapping[] ConvertToOpenImplementationArgumentMappingsRecursive(
            ArgumentMapping mapping, IList<Type> processedTypes)
        {
            // If we're dealing with a generic type argument here (which means we're in the verification
            // phase testing open generic types), we must just return the mapping, because we can't deduce
            // things any further.
            if (mapping.ConcreteType.IsGenericParameter)
            {
                return new ArgumentMapping[] { mapping };
            }

            var argumentTypeDefinition = mapping.Argument.GetGenericTypeDefinition();

            // Try to get mappings for each type in the type hierarchy that is compatible to the  argument.
            return (
                from type in mapping.ConcreteType.GetTypeBaseTypesAndInterfacesFor(argumentTypeDefinition)
                from arg in ConvertToOpenImplementationArgumentMappingsForType(mapping, type, processedTypes)
                select arg)
                .ToArray();
        }

        private ArgumentMapping[] ConvertToOpenImplementationArgumentMappingsForType(
           ArgumentMapping mapping, Type type, IList<Type> processedTypes)
        {
            var arguments = mapping.Argument.GetGenericArguments();
            var concreteTypes = type.GetGenericArguments();

            if (concreteTypes.Length != arguments.Length)
            {
                // The length of the concrete list and the generic argument list does not match. This normally
                // means that the generic argument contains a argument that is not generic (so Int32 instead
                // of T). In that case we can ignore everything, because the type will be unusable.
                return Helpers.Array<ArgumentMapping>.Empty;
            }

            return (
                from subMapping in ArgumentMapping.Zip(arguments, concreteTypes)
                from arg in ConvertToOpenImplementationArgumentMappings(subMapping, processedTypes).ToArray()
                select arg)
                .ToArray();
        }
    }
}