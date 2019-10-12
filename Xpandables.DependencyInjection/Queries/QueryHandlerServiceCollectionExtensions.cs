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
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
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

            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerValidationDecorator{TQuery, TResult}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQueryValidationDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerPersistenceDecorator{TQuery, TResult}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQueryPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistenceDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerTransactionDecorator{TQuery, TResult}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQueryTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerTransactionDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerEventRegisterDecorator{TQuery, TResult}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQueryEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerEventRegisterDecorator<,>));
            return services;
        }
    }
}
