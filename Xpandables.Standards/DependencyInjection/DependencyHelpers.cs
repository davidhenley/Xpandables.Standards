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
using System.Design.Mediator;
using System.Design.Query;
using System.Design.TaskEvent;
using System.Linq;
using System.Reflection;

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
        /// Adds the <see cref="ICommandQueryHandler"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCommandQueryHandler(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ICommandQueryHandler, CommandQueryHandler>();
            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorate<ICommandQueryHandler, CommandQueryHandlerValidator>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncCommandQueryHandler"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncCommandQueryHandler(
            this IServiceCollection services,
            DecorateWith decorateWith = DecorateWith.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAsyncCommandQueryHandler, AsyncCommandQueryHandler>();
            if (decorateWith.HasFlag(DecorateWith.Validation))
                services.TryDecorate<IAsyncCommandQueryHandler, AsyncCommandQueryHandlerValidator>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IQueryHandler{TQuery, TResult}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
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
        /// Adds the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
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
        /// Adds the <see cref="ICommandHandler{TCommand}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
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
        /// Adds the <see cref="IAsyncCommandHandler{TCommand}"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="decorateWith">The decorator to be added with.</param>
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
        /// Adds the <see cref="ITaskEventHandler{TTaskEvent}"/> to the services.
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
                .AddClasses(classes => classes.AssignableTo(typeof(ITaskEventHandler<>))
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
        /// Adds the <see cref="ITaskEventRegister"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddTaskEventRegister(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<TaskEventRegister>();
            services.AddScoped<ITaskEventRegister>(provider => provider.GetRequiredService<TaskEventRegister>());
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncTaskEventRegister"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncTaskEventRegister(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<AsyncTaskEventRegister>();
            services.AddScoped<IAsyncTaskEventRegister>(provider => provider.GetRequiredService<AsyncTaskEventRegister>());
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ITaskEventDispatcher"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddTaskEventDispatcher(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ITaskEventDispatcher, TaskEventDispatcher>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncTaskEventDispatcher"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddAsyncTaskEventDispatcher(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAsyncTaskEventDispatcher, AsyncTaskEventDispatcher>();
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
