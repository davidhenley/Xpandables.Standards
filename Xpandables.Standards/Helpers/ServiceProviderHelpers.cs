/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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
#pragma warning disable CS1591
    /// <summary>
    /// Extension methods for getting services from an <see cref="IServiceProvider" />.
    /// </summary>
    public static class ServiceProviderHelpers
    {
        public static TService GetService<TService>(this IServiceProvider serviceProvider)
            where TService : class => serviceProvider.GetService(typeof(TService)) as TService;

        public static TService GetService<TService>(this IServiceProvider serviceProvider, Type serviceType)
            where TService : class => serviceProvider.GetService(serviceType) as TService;

        public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, Type serviceType)
            => serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }))
                as IEnumerable<object>;

        public static IEnumerable<TService> GetServices<TService>(this IServiceProvider serviceProvider)
            where TService : class
            => serviceProvider.GetService(typeof(IEnumerable<TService>)) as IEnumerable<TService>;

        public static IEnumerable<TService> GetServices<TService>(
            this IServiceProvider serviceProvider, Type serviceType)
            where TService : class
            => serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(new Type[] { serviceType }))
                as IEnumerable<TService>;
    }
#pragma warning restore CS1591
}