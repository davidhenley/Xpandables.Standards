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
using System.Design.TaskEvent;
using System.Linq;
using System.Reflection;
using System;

namespace System
{
    public static class DependencyHelpers
    {
        /// <summary>
        /// Adds the <see cref="ICorrelationContext"/> to the services.
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
        /// Adds the <see cref="IConfigurationAccessor"/> to the services.
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
        /// Adds the <see cref="IStringEncryptor"/> and <see cref="IStringGenerator"/> to the services.
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
        /// Adds the <see cref="IProcessor"/> to the services.
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
                services.TryDecorate<IProcessor, ProcessorValidator>();
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate<IProcessor, ProcessorPersistence>();
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate<IProcessor, ProcessorTransaction>();
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate<IProcessor, ProcessorTaskEventRegister>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncProcessor"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncProcessor(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAsyncProcessor, AsyncProcessor>();
            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorate<IAsyncProcessor, AsyncProcessorValidator>();
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate<IAsyncProcessor, AsyncProcessorPersistence>();
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate<IAsyncProcessor, AsyncProcessorTransaction>();
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate<IAsyncProcessor, AsyncProcessorTaskEventRegister>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContext"/> to the services.
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
                    .Cast<IDataContext>();
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncDataContext"/> provider to the services.
        /// </summary>
        /// <typeparam name="TAsyncDataContextProvider">The type of async data context provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncDataContext<TAsyncDataContextProvider>(this IServiceCollection services)
            where TAsyncDataContextProvider : class, IAsyncDataContextProvider
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAsyncDataContextProvider, TAsyncDataContextProvider>();
            services.AddScoped(serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IAsyncDataContextProvider>();

                return AsyncHelpers.RunSync(() => dataContextProvider.GetDataContextAsync())
                    .WhenException(exception => throw new InvalidOperationException(
                        ErrorMessageResources.DataContextProviderException,
                        exception))
                    .Cast<IAsyncDataContext>();
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IQueryHandler{TQuery, TResult}"/> to the services.
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
                services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerValidator<,>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistence<,>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerTransaction<,>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerTaskEventRegister<,>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerValidator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddQueryValidatorDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerValidator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerPersistence{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddQueryPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerPersistence<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerTransaction{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddQueryTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerTransaction<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="QueryHandlerTaskEventRegister{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddQueryTaskEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerTaskEventRegister<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddAsyncQueryHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.AddScoped(typeof(AsyncQueryHandlerWrapper<,>));
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAsyncQueryHandler<,>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerValidator<,>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerPersistence<,>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerTransaction<,>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerTaskEventRegister<,>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncQueryHandlerValidator{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncQueryValidatorDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerValidator<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncQueryHandlerPersistence{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncQueryPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerPersistence<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncQueryHandlerTransaction{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncQueryTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerTransaction<,>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncQueryHandlerTaskEventRegister{TQuery, TResult}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncQueryTaskEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryHandlerTaskEventRegister<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICommandHandler{TCommand}"/> to the services.
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
                services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerValidator<>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerPersistence<>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTransaction<>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTaskEventRegister<>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerValidator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandValidatorDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerValidator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerPersistence{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerPersistence<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerTransaction{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTransaction<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="CommandHandlerTaskEventRegister{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandTaskEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerTaskEventRegister<>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncCommandHandler{TCommand}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with or use specific method.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandHandlers(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None,
            params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAsyncCommandHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerValidator<>));
            if (decorateWith.HasFlag(DecorateWith.Persistence))
                services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerPersistence<>));
            if (decorateWith.HasFlag(DecorateWith.Transaction))
                services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerTransaction<>));
            if (decorateWith.HasFlag(DecorateWith.EventRegister))
                services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerTaskEventRegister<>));

            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncCommandHandlerValidator{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandValidatorDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerValidator<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncCommandHandlerPersistence{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandPersistenceDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerPersistence<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncCommandHandlerTransaction{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandTransactionDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerTransaction<>));
            return services;
        }

        /// <summary>
        /// Adds <see cref="AsyncCommandHandlerTaskEventRegister{TCommand}"/> decorator.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandTaskEventRegisterDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.TryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandHandlerTaskEventRegister<>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEventHandler{TTaskEvent}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddTaskEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncTaskEventHandler{TTaskEvent}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddAsyncTaskEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies is null || !assemblies.Any()) throw new ArgumentNullException(nameof(assemblies));

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAsyncTaskEventHandler<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICorrelationTaskRegister"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddTaskEventRegister(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationTaskRegister>();
            services.AddScoped<ICorrelationTaskRegister>(provider => provider.GetRequiredService<CorrelationTaskRegister>());
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncCorrelationEventRegister"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncTaskEventRegister(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<AsyncTaskEventRegister>();
            services.AddScoped<IAsyncCorrelationEventRegister>(provider => provider.GetRequiredService<AsyncTaskEventRegister>());
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEventDispatcher"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddTaskEventDispatcher(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IEventDispatcher, Design.TaskEvent.TaskEventDispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEventDispatcher"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncTaskEventDispatcher(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IEventDispatcher, EventDispatcher>();
            return services;
        }


        /// <summary>
        /// Adds the <see cref="ICustomValidator{TArgument}"/> to the services.
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
