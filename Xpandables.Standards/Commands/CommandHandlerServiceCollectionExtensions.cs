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
using System.Design.Command;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register command handler services.
    /// </summary>
    public static class CommandHandlerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXCommandHandlers(
            this IServiceCollection services,
            Decorators decorateWith = Decorators.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if ((decorateWith & Decorators.Validation) == Decorators.Validation)
                services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
            if ((decorateWith & Decorators.Persistence) == Decorators.Persistence)
                services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerPersistenceDecorator<>));
            if ((decorateWith & Decorators.Transaction) == Decorators.Transaction)
                services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTransactionDecorator<>));
            if ((decorateWith & Decorators.EventRegister) == Decorators.EventRegister)
                services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerValidationDecorator{TCommand}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandValidationDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerPersistenceDecorator{TCommand}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerPersistenceDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerTransactionDecorator{TCommand}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTransactionDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerEventRegisterDecorator{TCommand}"/> decorator to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCommandEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));
            return services;
        }
    }
}
