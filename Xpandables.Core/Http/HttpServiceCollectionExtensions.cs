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

using System.Http;
using Microsoft.Extensions.DependencyInjection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register HTTP services
    /// </summary>
    public static class HttpServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default <see cref="IHttpRequestHeaderValuesAccessor"/>, <see cref="IHttpRequestTokenAccessor"/> and <see cref="IHttpRequestUserClaimAccessor"/>
        /// to the services with scoped life time.
        /// You can use specific extension methods to customize the behavior.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpServices(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddXHttpRequestHeaderValuesAccessor();
            services.AddXHttpRequestTokenAccessor();
            services.AddXHttpRequestUserClaimAccessor();
            return services;
        }

        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpRequestHeaderValuesAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpRequestHeaderValuesAccessor">The type of HTTP request header.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRequestHeaderValuesAccessor<THttpRequestHeaderValuesAccessor>(
            this IServiceCollection services)
            where THttpRequestHeaderValuesAccessor : class, IHttpRequestHeaderValuesAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IHttpRequestHeaderValuesAccessor, THttpRequestHeaderValuesAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the default HTTP request header values accessor that implements the <see cref="IHttpRequestHeaderValuesAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRequestHeaderValuesAccessor(this IServiceCollection services)
            => services.AddXHttpRequestHeaderValuesAccessor<HttpRequestHeaderValuesAccessor>();
    }
}
