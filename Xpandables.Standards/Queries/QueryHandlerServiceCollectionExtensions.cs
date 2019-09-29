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
using System.Design.Query;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register query handler.
    /// </summary>
    public static class QueryHandlerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IQueryHandler{TQuery, TResult}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddCustomQueryHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.AddTransient(typeof(QueryHandlerWrapper<,>));
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if ((decorateWith & DecorateWith.Validation) == DecorateWith.Validation)
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
            if ((decorateWith & DecorateWith.Persistence) == DecorateWith.Persistence)
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistenceDecorator<,>));
            if ((decorateWith & DecorateWith.Transaction) == DecorateWith.Transaction)
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerTransactionDecorator<,>));
            if ((decorateWith & DecorateWith.EventRegister) == DecorateWith.EventRegister)
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerEventRegisterDecorator<,>));
            if ((decorateWith & DecorateWith.Logging) == DecorateWith.Logging)
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerValidationDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomQueryValidationDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerPersistenceDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomQueryPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerTransactionDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomQueryTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerTransactionDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerEventRegisterDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomQueryEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerEventRegisterDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerLoggingDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomQueryLoggingDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));
            return services;
        }
    }
}
