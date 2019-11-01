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
using System.Design.Database;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register data context services.
    /// </summary>
    public static class DataContextServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDataContext"/> accessor to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContextAccessor">The type of data context accessor
        /// that implements <see cref="IDataContextAccessor"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContextAccessor>(this IServiceCollection services)
            where TDataContextAccessor : class, IDataContextAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IDataContextAccessor, TDataContextAccessor>();
            services.AddScoped(serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IDataContextAccessor>();
                return dataContextProvider.GetDataContext()
                    .WhenException(exception => throw new InvalidOperationException(
                        ErrorMessageResources.DataContextProviderException,
                        exception))
                    .Cast<IDataContext>();
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextSeeder"/> to the services with scoped life time that will be used to seed every data context
        /// that it's decorated with the <see cref="ISeederBehavior"/> interface.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextSeederBehavior<TDataContextSeeder>(this IServiceCollection services)
            where TDataContextSeeder : class, IDataContextSeeder
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IDataContextSeeder, TDataContextSeeder>();
            services.XTryDecorate<IDataContextAccessor, DataContextSeederBehavior>();
            return services;
        }
    }
}
