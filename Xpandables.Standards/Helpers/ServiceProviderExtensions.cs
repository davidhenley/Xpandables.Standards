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
    /// <summary>
    /// Extension methods for getting services from a <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <returns>An instance of optional with found service otherwise empty.</returns>
        public static Optional<TService> GetServiceExtended<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return serviceProvider.GetService(typeof(TService)).AsOptional().CastOptional<TService>();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found service otherwise empty.</returns>
        public static Optional<TService> GetServiceExtended<TService>(this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return serviceProvider.GetService(serviceType).AsOptional().CastOptional<TService>();
        }

        /// <summary>
        /// Gets the services object of the specified type.
        /// If not found, returns an <see cref="Enumerable.Empty{T}"/>.
        /// </summary>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found services otherwise empty.</returns>
        public static IEnumerable<object> GetServicesExtended(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider
                .GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }))
                is IEnumerable<object> services)
            {
                return services;
            }

            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Gets the services object of the specified type.
        /// If not found, returns an <see cref="Enumerable.Empty{TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <returns>An instance of optional with found services otherwise empty.</returns>
        public static IEnumerable<TService> GetServicesExtended<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService(typeof(IEnumerable<TService>)) is IEnumerable<TService> services)
                return services;

            return Enumerable.Empty<TService>(); ;
        }

        /// <summary>
        /// Gets the services object of the specified type.
        /// If not found, returns an <see cref="Enumerable.Empty{TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found services otherwise empty.</returns>
        public static IEnumerable<TService> GetServicesExtended<TService>(
            this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider
                .GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }))
                is IEnumerable<TService> services)
            {
                return services;
            }

            return Enumerable.Empty<TService>();
        }

        /// <summary>
        /// Instantiates a type with constructor provided directly and/or from an System.IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act with.</param>
        /// <param name="type">The target type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static Optional<object> GetServiceOrCreateInstance(this IServiceProvider serviceProvider, Type type)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            try
            {
                return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        /// <summary>
        /// Instantiates a type with constructor arguments provided directly and/or from an System.IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act with.</param>
        /// <param name="type">The target type.</param>
        /// <param name="arguments">The optional arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static Optional<object> CreateInstance(this IServiceProvider serviceProvider, Type type, params object[] arguments)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            if (type is null) throw new ArgumentNullException(nameof(type));

            try
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, type, arguments);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        /// <summary>
        /// Instantiates a type with constructor provided from service descriptor and/or from an System.IServiceProvider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act with.</param>
        /// <param name="descriptor">The service descriptor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="descriptor"/> is null.</exception>
        public static Optional<object> GetInstance(this IServiceProvider serviceProvider, ServiceDescriptor descriptor)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationType != null)
            {
                return serviceProvider.GetServiceOrCreateInstance(descriptor.ImplementationType);
            }

            try
            {
                return descriptor.ImplementationFactory(serviceProvider);
            }
            catch (Exception exception)
            {
                return Optional<object>.Exception(exception);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ServiceDescriptor"/> with the specified serviceType and implementationFactory.
        /// </summary>
        /// <param name="descriptor">The descriptor to act on.</param>
        /// <param name="factory">The factory to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="descriptor"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is null.</exception>
        public static Optional<ServiceDescriptor> WithFactory(
            this ServiceDescriptor descriptor,
            Func<IServiceProvider, object> factory)
        {
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            try
            {
                return ServiceDescriptor.Describe(descriptor.ServiceType, factory, descriptor.Lifetime);
            }
            catch (Exception exception)
            {
                return Optional<ServiceDescriptor>.Exception(exception);
            }
        }
    }
}