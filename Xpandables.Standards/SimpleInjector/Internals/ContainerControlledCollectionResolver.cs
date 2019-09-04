﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SimpleInjector;
    using SimpleInjector.Decorators;

    internal sealed class ContainerControlledCollectionResolver : CollectionResolver
    {
        internal ContainerControlledCollectionResolver(Container container, Type openGenericServiceType)
            : base(container, openGenericServiceType)
        {
        }

        internal override void RegisterUncontrolledCollection(Type serviceType, InstanceProducer producer)
        {
            throw new NotSupportedException(
                StringResources.MixingRegistrationsWithControlledAndUncontrolledIsNotSupported(serviceType,
                    controlled: false));
        }

        internal override void AddControlledRegistrations(
            Type serviceType, ContainerControlledItem[] items, bool append)
        {
            var group = RegistrationGroup.CreateForControlledItems(serviceType, items, append);
            AddRegistrationGroup(group);
        }

        protected override InstanceProducer BuildCollectionProducer(Type closedServiceType)
        {
            ContainerControlledItem[] closedGenericImplementations =
                GetClosedContainerControlledItemsFor(closedServiceType);

            IContainerControlledCollection collection =
                ControlledCollectionHelper.CreateContainerControlledCollection(
                    closedServiceType, Container);

            collection.AppendAll(closedGenericImplementations);

            var collectionType = typeof(IEnumerable<>).MakeGenericType(closedServiceType);

            return new InstanceProducer(
                serviceType: collectionType,
                registration: collection.CreateRegistration(collectionType, Container));
        }

        protected override Type[] GetAllKnownClosedServiceTypes() => (
            from registrationGroup in RegistrationGroups
            from item in registrationGroup.ControlledItems
            let implementation = item.ImplementationType
            where !implementation.ContainsGenericParameters
            from service in implementation.GetTypeBaseTypesAndInterfacesFor(ServiceType)
            select service)
            .Distinct()
            .ToArray();

        private ContainerControlledItem[] GetClosedContainerControlledItemsFor(Type serviceType)
        {
            var items = GetItemsFor(serviceType);

            return serviceType.IsGenericType
                ? GetClosedGenericImplementationsFor(serviceType, items)
                : items.ToArray();
        }

        private IEnumerable<ContainerControlledItem> GetItemsFor(Type closedGenericServiceType) =>
            from registrationGroup in RegistrationGroups
            where registrationGroup.ServiceType.ContainsGenericParameters ||
                closedGenericServiceType.IsAssignableFrom(registrationGroup.ServiceType)
            from item in registrationGroup.ControlledItems
            select item;

        private static ContainerControlledItem[] GetClosedGenericImplementationsFor(
            Type closedGenericServiceType, IEnumerable<ContainerControlledItem> containerControlledItems)
        {
            return (
                from item in containerControlledItems
                let openGenericImplementation = item.ImplementationType
                let builder = new GenericTypeBuilder(closedGenericServiceType, openGenericImplementation)
                let result = builder.BuildClosedGenericImplementation()
                where result.ClosedServiceTypeSatisfiesAllTypeConstraints
                select item.Registration != null
                    ? item
                    : ContainerControlledItem.CreateFromType(
                        openGenericImplementation, result.ClosedGenericImplementation!))
                .ToArray();
        }
    }
}