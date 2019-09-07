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
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Design.Command;
using System.Design.Database;
using System.Design.Mediator;
using System.Design.Query;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    public static class PatternServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
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
        public static IServiceCollection AddConfigurationAccessor(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IConfigurationAccessor, ConfigurationAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IStringEncryptor"/> and <see cref="IStringGenerator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddStringEncryptorGenerator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IStringEncryptor, StringEncryptor>();
            services.AddTransient<IStringGenerator, StringGenerator>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IProcessor"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddProcessor(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IProcessor, Processor>();
            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorateExtended<IProcessor, ProcessorValidationDecorator>();
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorateExtended<IProcessor, ProcessorPersistenceDecorator>();
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorateExtended<IProcessor, ProcessorTransactionDecorator>();
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorateExtended<IProcessor, ProcessorEventRegisterDecorator>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TDataContextProvider">The type of data context provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddDataContext<TDataContextProvider>(this IServiceCollection services)
            where TDataContextProvider : class, IDataContextProvider
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataContextProvider, TDataContextProvider>();
            services.AddScoped(serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IDataContextProvider>();
                return dataContextProvider.GetDataContext()
                    .WhenException(exception => throw new InvalidOperationException(
                        ErrorMessageResources.DataContextProviderException,
                        exception))
                    .Return();
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IQueryHandler{TQuery, TResult}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddQueryHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.AddTransient(typeof(QueryHandlerWrapper<,>));
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistenceDecorator<,>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerTransactionDecorator<,>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerEventRegisterDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerValidationDecorator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddQueryValidationDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddQueryPersistenceDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddQueryTransactionDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddQueryEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(IQueryHandler<,>), typeof(QueryHandlerEventRegisterDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddCommandHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerPersistenceDecorator<>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerTransactionDecorator<>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerValidationDecorator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandValidatorDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddCommandPersistenceDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddCommandTransactionDecorator(this IServiceCollection services)
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
        public static IServiceCollection AddCommandEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorateExtended(typeof(ICommandHandler<>), typeof(CommandHandlerEventRegisterDecorator<>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICorrelationTaskRegister"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCorrelationTaskRegister(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationTaskRegister>();
            services.AddScoped<ICorrelationTaskRegister>(provider => provider.GetRequiredService<CorrelationTaskRegister>());
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICustomValidator{TArgument}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.AddTransient(typeof(ICustomCompositeValidator<>), typeof(CustomCompositeValidator<>));
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICustomValidator<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }
    }

    [Flags]
    public enum DecorateWith
    {
        None,
        Persistence,
        EventRegister,
        Transaction,
        Validation,
        Logging
    }
}
