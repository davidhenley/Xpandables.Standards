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

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register specific services.
    /// </summary>
    public static class SpecificServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IStringEncryptor"/>, <see cref="IStringGenerator"/> and
        /// <see cref="IStringGeneratorEncryptor"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXStringEncryptorGenerator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IStringEncryptor, StringEncryptor>();
            services.AddTransient<IStringGenerator, StringGenerator>();
            services.AddTransient<IStringGeneratorEncryptor, StringGeneratorEncryptor>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDateTimeEngine"/> that implements the <see cref="IDateTimeEngine"/> interface
        /// with transient life time.
        /// </summary>
        /// <typeparam name="TDateTimeEngine">The type of date time provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDatTimeEngine<TDateTimeEngine>(this IServiceCollection services)
            where TDateTimeEngine : class, IDateTimeEngine
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDateTimeEngine, TDateTimeEngine>();
            return services;
        }

        /// <summary>
        /// Adds the default date time engine implementation to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDatTimeEngine(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDateTimeEngine, DateTimeEngine>();
            return services;
        }

        /// <summary>
        /// Adds the instance creator to the services with the transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="withCache">Determines whether or not the instance creator should be used with cache.
        /// The default value is false.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXInstanceCreator(this IServiceCollection services, bool withCache = false)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (withCache)
                services.AddTransient<IInstanceCreator, InstanceCreatorCache>();
            else
                services.AddTransient<IInstanceCreator, InstanceCreator>();
            return services;
        }
    }
}
