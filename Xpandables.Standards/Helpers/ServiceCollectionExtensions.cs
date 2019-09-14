/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace System.Design.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ensures that the supplied <typeparamref name="TDecorator"/> decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <typeparamref name="TDecorator"/>. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <typeparamref name="TDecorator"/>
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <typeparamref name="TDecorator"/>.</typeparam>
        /// <typeparam name="TDecorator">The decorator type that will be used to wrap the original service type.
        /// </typeparam>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended<TService, TDecorator>(this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            return services.TryDecorateExtended(typeof(TService), typeof(TDecorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</typeparam>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended<TService>(
            this IServiceCollection services,
            Func<TService, IServiceProvider, TService> decorator)
            where TService : class
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(
                typeof(TService),
                serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="serviceType">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended(
            this IServiceCollection services,
            Type serviceType,
            Func<object, IServiceProvider, object> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(
                serviceType,
                serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</typeparam>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended<TService>(
            this IServiceCollection services,
            Func<TService, TService> decorator)
            where TService : class
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(
                typeof(TService),
                serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="serviceType">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended(
            this IServiceCollection services,
            Type serviceType,
            Func<object, object> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(
                serviceType,
                serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }


        /// <summary>
        /// Ensures that the supplied <paramref name="decoratorType"/> decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decoratorType"/>. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decoratorType"/>
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="serviceType">The service type that will be wrapped by the given decorator.</param>
        /// <param name="decoratorType">The decorator type that will be used to wrap the original service type.</param>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decoratorType"/> argument is <c>null</c>.</exception>
        public static IServiceCollection TryDecorateExtended(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (decoratorType is null) throw new ArgumentNullException(nameof(decoratorType));

            return serviceType.IsOpenGeneric() && decoratorType.IsOpenGeneric()
                ? services.DecorateOpenGenerics(serviceType, decoratorType)
                : services.DecorateDescriptors(
                    serviceType,
                    serviceDescriptor => serviceDescriptor.DecorateDescriptor(decoratorType));
        }


        private static IServiceCollection DecorateOpenGenerics(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType)
        {
            var serviceArguments = services.GetArgumentTypes(serviceType);

            serviceArguments
                .Map(arguments =>
                {
                    foreach (var argument in arguments)
                    {
                        var closedServiceType = serviceType.MakeGenericTypeSafe(argument);
                        var closedDecoratorType = decoratorType.MakeGenericTypeSafe(argument);
                        var closedServiceDecoratorType = closedServiceType.AndOptional(() => closedDecoratorType);

                        closedServiceDecoratorType
                            .Map(serviceDecoratorType =>
                                services.DecorateDescriptors(
                                    serviceDecoratorType.Left,
                                    descriptor => descriptor.DecorateDescriptor(serviceDecoratorType.Right)));
                    }
                });

            return services;
        }

        private static IServiceCollection DecorateDescriptors(
            this IServiceCollection services,
            Type serviceType,
            Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            var serviceDescriptors = services.GetServiceDescriptors(serviceType);

            serviceDescriptors
                .Map(descriptors =>
                {
                    foreach (var descriptor in descriptors)
                    {
                        var index = services.IndexOf(descriptor);
                        services[index] = decorator(descriptor);
                    }
                });

            return services;
        }

        private static Optional<ICollection<Type[]>> GetArgumentTypes(
            this IServiceCollection services,
            Type serviceType)
            => services
                .Where(x => !x.ServiceType.IsGenericTypeDefinition && IsSameGenericType(x.ServiceType, serviceType))
                .Select(x => x.ServiceType.GenericTypeArguments)
                .ToArray();

        private static bool IsSameGenericType(Type t1, Type t2)
            => t1.IsGenericType
                && t2.IsGenericType
                && t1.GetGenericTypeDefinition() == t2.GetGenericTypeDefinition();

        private static Optional<ICollection<ServiceDescriptor>> GetServiceDescriptors(
            this IServiceCollection services,
            Type serviceType)
            => services.Where(service => service.ServiceType == serviceType).ToArray();

        private static ServiceDescriptor DecorateDescriptor(
            this ServiceDescriptor descriptor,
            Type decoratorType)
            => descriptor.WithFactory(
                provider => provider.CreateInstance(
                    decoratorType,
                    new object[] { provider.GetInstance(descriptor) }));

        private static ServiceDescriptor DecorateDescriptor<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, IServiceProvider, TService> decorator)
            where TService : class
            => descriptor.WithFactory(
                provider => decorator(
                    provider.GetInstance(descriptor).Cast<TService>(),
                    provider));

        private static ServiceDescriptor DecorateDescriptor<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, TService> decorator)
            where TService : class
            => descriptor.WithFactory(
                provider => decorator(provider.GetInstance(descriptor).Cast<TService>()));
    }
}
