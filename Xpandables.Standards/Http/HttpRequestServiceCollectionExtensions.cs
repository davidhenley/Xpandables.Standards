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
using System.Http;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register HTTP services
    /// </summary>
    public static class HttpRequestServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpRequestHeaderValuesAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpRequestHeaderValuesAccessor">The type of HTTP request header.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomHttpRequestHeaderValuesAccessor<THttpRequestHeaderValuesAccessor>(
            this IServiceCollection services)
            where THttpRequestHeaderValuesAccessor : class, IHttpRequestHeaderValuesAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IHttpRequestHeaderValuesAccessor, THttpRequestHeaderValuesAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the default HTTP request token accessor that implements the <see cref="IHttpRequestTokenAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomHttpRequestTokenAccessor(this IServiceCollection services)
            => services.AddCustomHttpRequestTokenAccessor<HttpRequestTokenAccessor>();

        /// <summary>
        /// Adds the specified HTTP request token accessor.
        /// The type should implement the <see cref="IHttpRequestTokenAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpRequestTokenAccessor">The type of HTTP request token.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomHttpRequestTokenAccessor<THttpRequestTokenAccessor>(
            this IServiceCollection services)
            where THttpRequestTokenAccessor : class, IHttpRequestTokenAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IHttpRequestTokenAccessor, THttpRequestTokenAccessor>();
            return services;
        }
    }
}
