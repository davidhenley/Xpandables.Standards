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
        /// Adds the <see cref="IDataContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContextProvider">The type of data context provider
        /// that implements <see cref="IDataContextProvider"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContextProvider>(this IServiceCollection services)
            where TDataContextProvider : class, IDataContextProvider
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IDataContextProvider, TDataContextProvider>();
            services.AddScoped(serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IDataContextProvider>();
                return dataContextProvider.GetDataContext()
                    .WhenException(exception => throw new InvalidOperationException(
                        ErrorMessageResources.DataContextProviderException,
                        exception))
                    .Cast<IDataContext>();
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextSeeder"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContextSeederDecorator<TDataContextSeeder>(this IServiceCollection services)
            where TDataContextSeeder : class, IDataContextSeeder
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IDataContextSeeder, TDataContextSeeder>();
            services.XTryDecorate<IDataContextProvider, DataContextProviderSeederDecorator>();
            return services;
        }
    }
}
