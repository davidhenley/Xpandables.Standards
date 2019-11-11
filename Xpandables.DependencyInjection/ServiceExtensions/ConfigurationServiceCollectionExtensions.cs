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

using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register configuration services.
    /// </summary>
    public static class ConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<ICorrelationContext, CorrelationContext>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IConfigurationAccessor"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXConfigurationAccessor(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IConfigurationAccessor, ConfigurationAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ITokenEngine"/> to the services with transient life time.
        /// </summary>
        /// <typeparam name="TTokenEngine">The type of that implements <see cref="ITokenEngine"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXTokenEngine<TTokenEngine>(this IServiceCollection services)
            where TTokenEngine : class, ITokenEngine
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<ITokenEngine, TTokenEngine>();
            return services;
        }   
    }
}
