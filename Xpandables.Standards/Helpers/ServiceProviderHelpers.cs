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

using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Extension methods for getting services from an <see cref="IServiceProvider" />.
    /// </summary>
    public static class ServiceProviderHelpers
    {
        public static TService GetService<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (TService)serviceProvider.GetService(typeof(TService));
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (TService)serviceProvider.GetService(serviceType);
        }

        public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (IEnumerable<object>)serviceProvider
                .GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }));
        }

        public static IEnumerable<TService> GetServices<TService>(this IServiceProvider serviceProvider)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (IEnumerable<TService>)serviceProvider.GetService(typeof(IEnumerable<TService>));
        }

        public static IEnumerable<TService> GetServices<TService>(
            this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
            return (IEnumerable<TService>)serviceProvider
                .GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }));
        }
    }
}