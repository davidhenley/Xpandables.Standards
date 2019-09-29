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
        public static IServiceCollection AddCustomCommandHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
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

            if ((decorateWith & DecorateWith.Validation) == DecorateWith.Validation)
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
            if ((decorateWith & DecorateWith.Persistence) == DecorateWith.Persistence)
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerPersistenceDecorator<>));
            if ((decorateWith & DecorateWith.Transaction) == DecorateWith.Transaction)
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerTransactionDecorator<>));
            if ((decorateWith & DecorateWith.EventRegister) == DecorateWith.EventRegister)
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));
            if ((decorateWith & DecorateWith.Logging) == DecorateWith.Logging)
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerValidationDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomCommandValidationDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerPersistenceDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomCommandPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerPersistenceDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerTransactionDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomCommandTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerTransactionDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerEventRegisterDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomCommandEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerLoggingDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomCommandLoggingDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));
            return services;
        }
    }
}
