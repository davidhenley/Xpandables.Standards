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
        /// Adds the default HTTP request header user claims accessor that implements the <see cref="IHttpRequestUserClaimAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRequestUserClaimAccessor(this IServiceCollection services)
            => services.AddScoped<IHttpRequestUserClaimAccessor, HttpRequestUserClaimAccessor>();

        /// <summary>
        /// Adds the default HTTP request header user claims accessor that implements the <see cref="IHttpRequestUserClaimAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpRequestUserClaimAccessor">The type of HTTP request header user claims.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHtppRequestUserClaimAccessor<THttpRequestUserClaimAccessor>(
            this IServiceCollection services)
            where THttpRequestUserClaimAccessor : class, IHttpRequestUserClaimAccessor
            => services.AddScoped<IHttpRequestUserClaimAccessor, THttpRequestUserClaimAccessor>();

        /// <summary>
        /// Adds the default HTTP request token accessor that implements the <see cref="IHttpRequestTokenAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRequestTokenAccessor(this IServiceCollection services)
            => services.AddXHttpRequestTokenAccessor<HttpRequestTokenAccessor>();

        /// <summary>
        /// Adds the specified HTTP request token accessor.
        /// The type should implement the <see cref="IHttpRequestTokenAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpRequestTokenAccessor">The type of HTTP request token.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpRequestTokenAccessor<THttpRequestTokenAccessor>(
            this IServiceCollection services)
            where THttpRequestTokenAccessor : class, IHttpRequestTokenAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<IHttpRequestTokenAccessor, THttpRequestTokenAccessor>();
            return services;
        }

        /// <summary>
        /// Adds a delegate that will be used to add the authorization token before request execution
        /// using the <see cref="IHttpRequestTokenAccessor"/>.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IHttpClientBuilder.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        public static IHttpClientBuilder ConfigureXPrimaryAuthorizationTokenHandler(this IHttpClientBuilder builder)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var httpTokenProvider = provider.GetRequiredService<IHttpRequestTokenAccessor>();
                    return new HttpAuthorizationTokenHandler(httpTokenProvider);
                });

            return builder;
        }

        /// <summary>
        /// Adds a delegate that will be used to add the authorization token before request execution
        /// using the token provider function.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IHttpClientBuilder.</param>
        /// <param name="tokenProvider">The delegate token provider to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenProvider"/> is null.</exception>
        public static IHttpClientBuilder ConfigureXPrimaryAuthorizationTokenHandler(
            this IHttpClientBuilder builder, Func<string, string> tokenProvider)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (tokenProvider is null) throw new ArgumentNullException(nameof(tokenProvider));

            builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpTokenProvider = new HttpRequestTokenAccessorBuilder(tokenProvider);
                return new HttpAuthorizationTokenHandler(httpTokenProvider);
            });

            return builder;
        }
    }
}
