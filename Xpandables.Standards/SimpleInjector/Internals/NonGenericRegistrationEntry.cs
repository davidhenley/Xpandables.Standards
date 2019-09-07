// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class NonGenericRegistrationEntry : IRegistrationEntry
    {
        private readonly List<IProducerProvider> providers = new List<IProducerProvider>(1);
        private readonly Type nonGenericServiceType;
        private readonly Container container;

        public NonGenericRegistrationEntry(Type nonGenericServiceType, Container container)
        {
            this.nonGenericServiceType = nonGenericServiceType;
            this.container = container;
        }

        private interface IProducerProvider
        {
            IEnumerable<InstanceProducer> CurrentProducers { get; }

            InstanceProducer? TryGetProducer(InjectionConsumerInfo consumer, bool handled);
        }

        public IEnumerable<InstanceProducer> CurrentProducers =>
            providers.SelectMany(p => p.CurrentProducers);

        private IEnumerable<InstanceProducer> ConditionalProducers =>
            CurrentProducers.Where(p => p.IsConditional);

        private IEnumerable<InstanceProducer> UnconditionalProducers =>
            CurrentProducers.Where(p => !p.IsConditional);

        public int GetNumberOfConditionalRegistrationsFor(Type serviceType) =>
            CurrentProducers.Count(p => p.IsConditional);

        public void Add(InstanceProducer producer)
        {
            container.ThrowWhenContainerIsLockedOrDisposed();
            ThrowWhenConditionalAndUnconditionalAreMixed(producer);
            ThrowWhenConditionalIsRegisteredInOverridingMode(producer);

            ThrowWhenTypeAlreadyRegistered(producer);
            ThrowWhenIdenticalImplementationIsAlreadyRegistered(producer);

            if (producer.IsUnconditional)
            {
                providers.Clear();
            }

            providers.Add(new SingleInstanceProducerProvider(producer));
        }

        public void Add(
            Type serviceType,
            Func<TypeFactoryContext, Type> implementationTypeFactory,
            Lifestyle lifestyle,
            Predicate<PredicateContext>? predicate)
        {
            Requires.IsNotNull(predicate, "only support conditional for now");

            container.ThrowWhenContainerIsLockedOrDisposed();

            if (UnconditionalProducers.Any())
            {
                throw new InvalidOperationException(
                    StringResources.NonGenericTypeAlreadyRegisteredAsUnconditionalRegistration(serviceType));
            }

            providers.Add(
                new ImplementationTypeFactoryInstanceProducerProvider(
                    serviceType,
                    implementationTypeFactory,
                    lifestyle,
                    predicate!,
                    container));
        }

        public InstanceProducer? TryGetInstanceProducer(Type serviceType, InjectionConsumerInfo consumer)
        {
            var instanceProducers = GetInstanceProducers(consumer).ToArray();

            if (instanceProducers.Length <= 1)
            {
                return instanceProducers.FirstOrDefault();
            }

            throw ThrowMultipleApplicableRegistrationsFound(instanceProducers);
        }

        public void AddGeneric(
            Type serviceType,
            Type implementationType,
            Lifestyle lifestyle,
            Predicate<PredicateContext>? predicate)
        {
            throw new NotSupportedException();
        }

        private IEnumerable<InstanceProducer> GetInstanceProducers(InjectionConsumerInfo consumer)
        {
            bool handled = false;

            foreach (var provider in providers)
            {
                InstanceProducer? producer = provider.TryGetProducer(consumer, handled);

                if (producer != null)
                {
                    yield return producer;
                    handled = true;
                }
            }
        }

        private void ThrowWhenTypeAlreadyRegistered(InstanceProducer producer)
        {
            if (producer.IsUnconditional
                && providers.Any()
                && !container.Options.AllowOverridingRegistrations)
            {
                throw new InvalidOperationException(
                    StringResources.TypeAlreadyRegistered(nonGenericServiceType));
            }
        }

        private void ThrowWhenIdenticalImplementationIsAlreadyRegistered(
            InstanceProducer producerToRegister)
        {
            // A provider overlaps the providerToRegister when it can be applied to ALL generic
            // types that the providerToRegister can be applied to as well.
            var overlappingProducers = GetOverlappingProducers(producerToRegister);

            bool isReplacement =
                producerToRegister.IsUnconditional && container.Options.AllowOverridingRegistrations;

            if (!isReplacement && overlappingProducers.Any())
            {
                var overlappingProducer = overlappingProducers.FirstOrDefault();

                throw new InvalidOperationException(
                    StringResources.AnOverlappingRegistrationExists(
                        producerToRegister.ServiceType,
                        overlappingProducer.ImplementationType,
                        overlappingProducer.IsConditional,
                        producerToRegister.ImplementationType,
                        producerToRegister.IsConditional));
            }
        }

        private IEnumerable<InstanceProducer> GetOverlappingProducers(InstanceProducer producerToRegister)
        {
            return
                from producer in CurrentProducers
                where producer.ImplementationType != null
                where !producer.Registration.WrapsInstanceCreationDelegate
                where !producerToRegister.Registration.WrapsInstanceCreationDelegate
                where producer.ImplementationType == producerToRegister.ImplementationType
                select producer;
        }

        private ActivationException ThrowMultipleApplicableRegistrationsFound(
            InstanceProducer[] instanceProducers)
        {
            var producersInfo =
                from producer in instanceProducers
                select Tuple.Create(nonGenericServiceType, producer.Registration.ImplementationType, producer);

            return new ActivationException(
                StringResources.MultipleApplicableRegistrationsFound(
                    nonGenericServiceType, producersInfo.ToArray()));
        }

        private void ThrowWhenConditionalAndUnconditionalAreMixed(InstanceProducer producer)
        {
            ThrowWhenNonGenericTypeAlreadyRegisteredAsUnconditionalRegistration(producer);
            ThrowWhenNonGenericTypeAlreadyRegisteredAsConditionalRegistration(producer);
        }

        private void ThrowWhenConditionalIsRegisteredInOverridingMode(InstanceProducer producer)
        {
            if (producer.IsConditional && container.Options.AllowOverridingRegistrations)
            {
                throw new NotSupportedException(
                    StringResources.MakingConditionalRegistrationsInOverridingModeIsNotSupported());
            }
        }

        private void ThrowWhenNonGenericTypeAlreadyRegisteredAsUnconditionalRegistration(
            InstanceProducer producer)
        {
            if (producer.IsConditional && UnconditionalProducers.Any())
            {
                throw new InvalidOperationException(
                    StringResources.NonGenericTypeAlreadyRegisteredAsUnconditionalRegistration(
                        producer.ServiceType));
            }
        }

        private void ThrowWhenNonGenericTypeAlreadyRegisteredAsConditionalRegistration(
            InstanceProducer producer)
        {
            if (producer.IsUnconditional && ConditionalProducers.Any())
            {
                throw new InvalidOperationException(
                    StringResources.NonGenericTypeAlreadyRegisteredAsConditionalRegistration(
                        producer.ServiceType));
            }
        }

        private sealed class SingleInstanceProducerProvider : IProducerProvider
        {
            private readonly InstanceProducer producer;

            public SingleInstanceProducerProvider(InstanceProducer producer)
            {
                this.producer = producer;
            }

            public IEnumerable<InstanceProducer> CurrentProducers => Enumerable.Repeat(producer, 1);

            public InstanceProducer? TryGetProducer(InjectionConsumerInfo consumer, bool handled) =>
                producer.Predicate(new PredicateContext(producer, consumer, handled))
                    ? producer
                    : null;
        }

        private class ImplementationTypeFactoryInstanceProducerProvider : IProducerProvider
        {
            private readonly Dictionary<Type, InstanceProducer> cache =
                new Dictionary<Type, InstanceProducer>();

            private readonly Func<TypeFactoryContext, Type> implementationTypeFactory;
            private readonly Lifestyle lifestyle;
            private readonly Predicate<PredicateContext> predicate;
            private readonly Type serviceType;
            private readonly Container container;

            public ImplementationTypeFactoryInstanceProducerProvider(
                Type serviceType,
                Func<TypeFactoryContext, Type> implementationTypeFactory,
                Lifestyle lifestyle,
                Predicate<PredicateContext> predicate,
                Container container)
            {
                this.serviceType = serviceType;
                this.implementationTypeFactory = implementationTypeFactory;
                this.lifestyle = lifestyle;
                this.predicate = predicate;
                this.container = container;
            }

            public IEnumerable<InstanceProducer> CurrentProducers
            {
                get
                {
                    lock (cache)
                    {
                        return cache.Values.ToArray();
                    }
                }
            }

            public InstanceProducer? TryGetProducer(InjectionConsumerInfo consumer, bool handled)
            {
                Func<Type> implementationTypeProvider =
                    () => GetImplementationTypeThroughFactory(consumer);

                var context =
                    new PredicateContext(serviceType, implementationTypeProvider, consumer, handled);

                // NOTE: The producer should only get built after it matches the delegate, to prevent
                // unneeded producers from being created, because this might cause diagnostic warnings,
                // such as torn lifestyle warnings.
                return predicate(context) ? GetProducer(context) : null;
            }

            private Type GetImplementationTypeThroughFactory(InjectionConsumerInfo consumer)
            {
                var context = new TypeFactoryContext(serviceType, consumer);

                Type implementationType = implementationTypeFactory(context);

                if (implementationType == null)
                {
                    throw new InvalidOperationException(
                        StringResources.FactoryReturnedNull(serviceType));
                }

                if (implementationType.ContainsGenericParameters)
                {
                    throw new ActivationException(
                        StringResources.TheTypeReturnedFromTheFactoryShouldNotBeOpenGeneric(
                            serviceType, implementationType));
                }

                Requires.FactoryReturnsATypeThatIsAssignableFromServiceType(
                    serviceType, implementationType);

                return implementationType;
            }

            private InstanceProducer GetProducer(PredicateContext context)
            {
                InstanceProducer producer;

                // Never build a producer twice. This could cause components with a torn lifestyle.
                lock (cache)
                {
                    // ImplementationType will never be null at this point.
                    Type implementationType = context.ImplementationType!;

                    // We need to cache on implementation, because service type is always the same.
                    if (!cache.TryGetValue(implementationType, out producer))
                    {
                        cache[implementationType] =
                            producer = CreateNewProducerFor(implementationType);
                    }
                }

                return producer;
            }

            private InstanceProducer CreateNewProducerFor(Type concreteType) =>
                new InstanceProducer(
                    serviceType,
                    lifestyle.CreateRegistration(concreteType, container),
                    predicate);
        }
    }
}