﻿/************************************************************************************************************
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

using System.Collections.Generic;
using System.Linq;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Extension methods for getting services from a <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderHelpers
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <returns>An instance of optional with found service otherwise empty.</returns>
        public static Optional<TService> XGetService<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (TService)serviceProvider.GetService(typeof(TService));
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// If not found, returns an empty optional.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found service otherwise empty.</returns>
        public static Optional<TService> XGetService<TService>(this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (TService)serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Gets the services object of the specified type.
        /// If not found, returns an <see cref="Enumerable.Empty{T}"/>.
        /// </summary>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found services otherwise empty.</returns>
        public static IEnumerable<object> XGetServices(this IServiceProvider serviceProvider, Type serviceType)
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
        public static IEnumerable<TService> XGetServices<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService(typeof(IEnumerable<TService>)) is IEnumerable<TService> services)
                return services;

            return Enumerable.Empty<TService>();
        }

        /// <summary>
        /// Gets the services object of the specified type.
        /// If not found, returns an <see cref="Enumerable.Empty{TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceProvider">The current service provider to use.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An instance of optional with found services otherwise empty.</returns>
        public static IEnumerable<TService> XGetServices<TService>(
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
    }
}