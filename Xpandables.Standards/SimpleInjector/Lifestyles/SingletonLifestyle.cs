﻿// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Lifestyles
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using SimpleInjector.Advanced;
    using SimpleInjector.Internals;

    internal sealed class SingletonLifestyle : Lifestyle
    {
        internal SingletonLifestyle() : base("Singleton")
        {
        }

        public override int Length => 1000;

        // This method seems somewhat redundant, since the exact same can be achieved by calling
        // Lifetime.Singleton.CreateRegistration(serviceType, () => instance, container). Calling that method
        // however, will trigger the ExpressionBuilding event later on where the supplied Expression is an
        // InvocationExpression calling that internally created Func<TService>. By having this extra method
        // (and the extra SingletonInstanceLifestyleRegistration class), we can ensure that the
        // ExpressionBuilding event is called with a ConstantExpression, which is much more intuitive to
        // anyone handling that event.
        internal static Registration CreateSingleInstanceRegistration(
            Type serviceType, object instance, Container container, Type? implementationType = null)
        {
            Requires.IsNotNull(instance, nameof(instance));

            return new SingletonInstanceLifestyleRegistration(
                serviceType, implementationType ?? serviceType, instance, container);
        }

        internal static InstanceProducer CreateUncontrolledCollectionProducer(
            Type itemType, IEnumerable collection, Container container) =>
            new InstanceProducer(
                typeof(IEnumerable<>).MakeGenericType(itemType),
                CreateUncontrolledCollectionRegistration(itemType, collection, container));

        internal static Registration CreateUncontrolledCollectionRegistration(
            Type itemType, IEnumerable collection, Container container)
        {
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);

            var registration = CreateSingleInstanceRegistration(enumerableType, collection, container);

            registration.IsCollection = true;

            return registration;
        }

        internal static bool IsSingletonInstanceRegistration(Registration registration) =>
            registration is SingletonInstanceLifestyleRegistration;

        protected internal override Registration CreateRegistrationCore<TConcrete>(Container container) =>
            new SingletonLifestyleRegistration<TConcrete>(container);

        protected internal override Registration CreateRegistrationCore<TService>(
            Func<TService> instanceCreator, Container container)
        {
            Requires.IsNotNull(instanceCreator, nameof(instanceCreator));

            return new SingletonLifestyleRegistration<TService>(container, instanceCreator);
        }

        private sealed class SingletonInstanceLifestyleRegistration : Registration
        {
            private readonly object locker = new object();

            private object instance;
            private bool initialized;

            internal SingletonInstanceLifestyleRegistration(
                Type serviceType, Type implementationType, object instance, Container container)
                : base(Lifestyle.Singleton, container)
            {
                this.instance = instance;
                ServiceType = serviceType;
                ImplementationType = implementationType;
            }

            public Type ServiceType { get; }
            public override Type ImplementationType { get; }

            public override Expression BuildExpression()
            {
                return Expression.Constant(GetInitializedInstance(), ImplementationType);
            }

            private object GetInitializedInstance()
            {
                if (!initialized)
                {
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            instance = GetInjectedInterceptedAndInitializedInstance();

                            initialized = true;
                        }
                    }
                }

                return instance;
            }

            private object GetInjectedInterceptedAndInitializedInstance()
            {
                try
                {
                    return GetInjectedInterceptedAndInitializedInstanceInternal();
                }
                catch (MemberAccessException ex)
                {
                    throw new ActivationException(
                        StringResources.UnableToResolveTypeDueToSecurityConfiguration(
                            ImplementationType, ex));
                }
            }

            private object GetInjectedInterceptedAndInitializedInstanceInternal()
            {
                Expression expression = Expression.Constant(instance, ImplementationType);

                // NOTE: We pass on producer.ServiceType as the implementation type for the following three
                // methods. This will the initialization to be only done based on information of the service
                // type; not on that of the implementation. Although now the initialization could be
                // incomplete, this behavior is consistent with the initialization of
                // Register<TService>(Func<TService>, Lifestyle), which doesn't have the proper static type
                // information available to use the implementation type.
                // TODO: This behavior should be reconsidered, because now it is incompatible with
                // Register<TService, TImplementation>(Lifestyle). So the question is, do we consider
                // RegisterInstance<TService>(TService) to be similar to Register<TService>(Func<TService>)
                // or to Register<TService, TImplementation>()? See: #353.
                expression = WrapWithPropertyInjector(ServiceType, expression);
                expression = InterceptInstanceCreation(ServiceType, expression);
                expression = WrapWithInitializer(ServiceType, expression);

                // Optimization: We don't need to compile a delegate in case all we have is a constant.
                if (expression is ConstantExpression constantExpression)
                {
                    return constantExpression.Value;
                }

                Delegate initializer = Expression.Lambda(expression).Compile();

                // This delegate might return a different instance than the originalInstance (caused by a
                // possible interceptor).
                return initializer.DynamicInvoke();
            }
        }

        private sealed class SingletonLifestyleRegistration<TImplementation> : Registration
            where TImplementation : class
        {
            private readonly object locker = new object();
            private readonly Func<TImplementation>? instanceCreator;

            private object? interceptedInstance;

            public SingletonLifestyleRegistration(
                Container container, Func<TImplementation>? instanceCreator = null)
                : base(Lifestyle.Singleton, container)
            {
                this.instanceCreator = instanceCreator;
            }

            public override Type ImplementationType => typeof(TImplementation);

            public override Expression BuildExpression() =>
                Expression.Constant(GetInterceptedInstance(), ImplementationType);

            private object GetInterceptedInstance()
            {
                // Even though the InstanceProducer takes a lock before calling Registration.BuildExpression
                // we need to take a lock here, because multiple InstanceProducer instances could reference
                // the same Registration and call this code in parallel.
                if (interceptedInstance == null)
                {
                    lock (locker)
                    {
                        if (interceptedInstance == null)
                        {
                            interceptedInstance = CreateInstanceWithNullCheck();

                            var disposable = interceptedInstance as IDisposable;

                            if (disposable != null && !SuppressDisposal)
                            {
                                Container.ContainerScope.RegisterForDisposal(disposable);
                            }
                        }
                    }
                }

                return interceptedInstance;
            }

            private TImplementation CreateInstanceWithNullCheck()
            {
                Expression expression =
                    instanceCreator == null
                        ? BuildTransientExpression()
                        : BuildTransientExpression(instanceCreator);

                Func<TImplementation> func = CompileExpression(expression);

                TImplementation instance = CreateInstance(func);

                EnsureInstanceIsNotNull(instance);

                return instance;
            }

            // Implements #553
            private TImplementation CreateInstance(Func<TImplementation> instanceCreator)
            {
                var isCurrentThread = new ThreadLocal<bool> { Value = true };

                // Create a listener that can spot when an injected stream is iterated during construction.
                Action<ServiceCreatedListenerArgs> listener = args =>
                {
                    // Only handle when an inner registration hasn't handled this yet.
                    if (!args.Handled)
                    {
                        // Only handle when the call originates from the same thread, as calls from different
                        // threads mean the listener is not triggered from this specific instanceCreator.
                        if (isCurrentThread.Value)
                        {
                            args.Handled = true;
                            var matchingRelationship = FindMatchingCollectionRelationship(args.Producer);

                            var additionalInformation = StringResources.CollectionUsedDuringConstruction(
                                typeof(TImplementation),
                                args.Producer,
                                matchingRelationship);

                            // At this point, an injected ContainerControlledCollection<T> has notified the
                            // listener about the creation of one of its elements. This has happened during
                            // the construction of this (Singleton) instance, which might cause Lifestyle
                            // Mismatches. That's why this is added as a known relationship. This way
                            // diagnostics can verify the relationship.
                            AddRelationship(
                                new KnownRelationship(
                                    implementationType: typeof(TImplementation),
                                    lifestyle: Lifestyle,
                                    consumer: matchingRelationship?.Consumer ?? InjectionConsumerInfo.Root,
                                    dependency: args.Producer,
                                    additionalInformation: additionalInformation));
                        }
                    }
                };

                try
                {
                    ControlledCollectionHelper.AddServiceCreatedListener(listener);

                    return instanceCreator();
                }
                finally
                {
                    ControlledCollectionHelper.RemoveServiceCreatedListener(listener);
                    isCurrentThread.Dispose();
                }
            }

            private KnownRelationship FindMatchingCollectionRelationship(
                InstanceProducer collectionItemProducer)
            {
                return (
                    from relationship in GetRelationships()
                    let producer = relationship.Dependency
                    where producer.IsContainerControlledCollection()
                    let controlledElementType = producer.GetContainerControlledCollectionElementType()
                    where controlledElementType == collectionItemProducer.ServiceType
                    select relationship)
                    .FirstOrDefault();
            }

            private static Func<TImplementation> CompileExpression(Expression expression)
            {
                try
                {
                    // Don't call BuildTransientDelegate, because that might do optimizations that are simply
                    // not needed, since the delegate will be called just once.
                    return CompilationHelpers.CompileLambda<TImplementation>(expression);
                }
                catch (Exception ex)
                {
                    string message = StringResources.ErrorWhileBuildingDelegateFromExpression(
                        typeof(TImplementation), expression, ex);

                    throw new ActivationException(message, ex);
                }
            }

            private static void EnsureInstanceIsNotNull(object instance)
            {
                if (instance is null)
                {
                    throw new ActivationException(
                        StringResources.DelegateForTypeReturnedNull(typeof(TImplementation)));
                }
            }
        }
    }
}