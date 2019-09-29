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
using System;
using System.Collections.Generic;
using System.Text;

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
        public static IServiceCollection AddCustomStringEncryptorGenerator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IStringEncryptor, StringEncryptor>();
            services.AddTransient<IStringGenerator, StringGenerator>();
            services.AddTransient<IStringGeneratorEncryptor, StringGeneratorEncryptor>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataTimeProvider"/> that implements the <see cref="IDateTimeProvider"/> interface.
        /// </summary>
        /// <typeparam name="TDataTimeProvider">The type of date time provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomDatTimeProvider<TDataTimeProvider>(this IServiceCollection services)
            where TDataTimeProvider:class, IDateTimeProvider
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDateTimeProvider, TDataTimeProvider>();
            return services;
        }

        /// <summary>
        /// Adds the default date time provider implementation.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomDatTimeProvider(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}
