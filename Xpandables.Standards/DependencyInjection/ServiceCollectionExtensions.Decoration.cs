/************************************************************************************************************
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Kristian Hellang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Design.DependencyInjection;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the specified type <typeparamref name="TDecorator"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of the type <typeparamref name="TService"/>
        /// has been registered.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
            where TDecorator : TService
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.DecorateDescriptors(typeof(TService), x => x.Decorate(typeof(TDecorator)));
        }

        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the specified type <typeparamref name="TDecorator"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        public static bool TryDecorate<TService, TDecorator>(this IServiceCollection services)
            where TDecorator : TService
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.TryDecorateDescriptors(typeof(TService), out _, x => x.Decorate(typeof(TDecorator)));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the specified <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decoratorType">The type to decorate existing services with.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of the specified
        /// <paramref name="serviceType"/> has been registered.</exception>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decoratorType"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Type decoratorType)
        {
            if (decoratorType is null) throw new ArgumentNullException(nameof(decoratorType));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            if (serviceType.IsOpenGeneric() && decoratorType.IsOpenGeneric())
            {
                return services.DecorateOpenGeneric(serviceType, decoratorType);
            }

            return services.DecorateDescriptors(serviceType, x => x.Decorate(decoratorType));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the specified <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decoratorType">The type to decorate existing services with.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decoratorType"/> arguments are <c>null</c>.</exception>
        public static bool TryDecorate(this IServiceCollection services, Type serviceType, Type decoratorType)
        {
            if (decoratorType is null) throw new ArgumentNullException(nameof(decoratorType));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            if (serviceType.IsOpenGeneric() && decoratorType.IsOpenGeneric())
            {
                return services.TryDecorateOpenGeneric(serviceType, decoratorType, out _);
            }

            return services.TryDecorateDescriptors(serviceType, out _, x => x.Decorate(decoratorType));
        }

        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <typeparam name="TService">The type of services to decorate.</typeparam>
        /// <param name="services">The services to add to.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of <typeparamref name="TService"/>
        /// has been registered.</exception>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Decorate<TService>(
            this IServiceCollection services,
            Func<TService, IServiceProvider, TService> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <typeparam name="TService">The type of services to decorate.</typeparam>
        /// <param name="services">The services to add to.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static bool TryDecorate<TService>(
            this IServiceCollection services,
            Func<TService, IServiceProvider, TService> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.TryDecorateDescriptors(typeof(TService), out _, x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <typeparam name="TService">The type of services to decorate.</typeparam>
        /// <param name="services">The services to add to.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of
        /// <typeparamref name="TService"/> has been registered.</exception>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Decorate<TService>(
            this IServiceCollection services,
            Func<TService, TService> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(typeof(TService), x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of type <typeparamref name="TService"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <typeparam name="TService">The type of services to decorate.</typeparam>
        /// <param name="services">The services to add to.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static bool TryDecorate<TService>(
            this IServiceCollection services,
            Func<TService, TService> decorator)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));

            return services.TryDecorateDescriptors(typeof(TService), out _, x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of the specified
        /// <paramref name="serviceType"/> has been registered.</exception>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Decorate(
            this IServiceCollection services,
            Type serviceType,
            Func<object, IServiceProvider, object> decorator)
        {
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.DecorateDescriptors(serviceType, x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static bool TryDecorate(
            this IServiceCollection services,
            Type serviceType,
            Func<object, IServiceProvider, object> decorator)
        {
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.TryDecorateDescriptors(serviceType, out _, x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="MissingTypeRegistrationException">If no service of the specified
        /// <paramref name="serviceType"/> has been registered.</exception>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection Decorate(
            this IServiceCollection services,
            Type serviceType,
            Func<object, object> decorator)
        {
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.DecorateDescriptors(serviceType, x => x.Decorate(decorator));
        }

        /// <summary>
        /// Decorates all registered services of the specified <paramref name="serviceType"/>
        /// using the <paramref name="decorator"/> function.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="serviceType">The type of services to decorate.</param>
        /// <param name="decorator">The decorator function.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>,
        /// <paramref name="serviceType"/> or <paramref name="decorator"/> arguments are <c>null</c>.</exception>
        public static bool TryDecorate(this IServiceCollection services, Type serviceType, Func<object, object> decorator)
        {
            if (decorator is null) throw new ArgumentNullException(nameof(decorator));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services.TryDecorateDescriptors(serviceType, out _, x => x.Decorate(decorator));
        }

        private static IServiceCollection DecorateOpenGeneric(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType)
        {
            if (services.TryDecorateOpenGeneric(serviceType, decoratorType, out var error))
            {
                return services;
            }

            throw error!;
        }

        private static bool IsSameGenericType(Type t1, Type t2)
        {
            return t1.IsGenericType && t2.IsGenericType && t1.GetGenericTypeDefinition() == t2.GetGenericTypeDefinition();
        }

        private static bool TryDecorateOpenGeneric(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType,
            out Exception? error)
        {
            var arguments = services
                .Where(x => !x.ServiceType.IsGenericTypeDefinition)
                .Where(x => IsSameGenericType(x.ServiceType, serviceType))
                .Select(x => x.ServiceType.GenericTypeArguments)
                .ToArray();

            if (arguments.Length == 0)
            {
                error = new MissingTypeRegistrationException(serviceType);
                return false;
            }

            foreach (var argument in arguments)
            {
                var closedServiceType = serviceType.MakeGenericType(argument);
                var closedDecoratorType = decoratorType.MakeGenericType(argument);

                if (!services.TryDecorateDescriptors(closedServiceType, out error, x => x.Decorate(closedDecoratorType)))
                {
                    return false;
                }
            }

            error = default;
            return true;
        }

        private static IServiceCollection DecorateDescriptors(
            this IServiceCollection services,
            Type serviceType,
            Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            if (services.TryDecorateDescriptors(serviceType, out var error, decorator))
            {
                return services;
            }

            throw error!;
        }

        private static bool TryDecorateDescriptors(
            this IServiceCollection services,
            Type serviceType,
            out Exception? error,
            Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            if (!services.TryGetDescriptors(serviceType, out var descriptors))
            {
                error = new MissingTypeRegistrationException(serviceType);
                return false;
            }

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);

                // To avoid reordering descriptors, in case a specific order is expected.
                services[index] = decorator(descriptor);
            }

            error = default;
            return true;
        }

        private static bool TryGetDescriptors(
            this IServiceCollection services,
            Type serviceType,
            out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }

        private static ServiceDescriptor Decorate<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, IServiceProvider, TService> decorator)
        {
            // TODO: Annotate TService with notnull when preview 8 is out.
            return descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor), provider)!);
        }

        private static ServiceDescriptor Decorate<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, TService> decorator)
        {
            // TODO: Annotate TService with notnull when preview 8 is out.
            return descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor))!);
        }

        private static ServiceDescriptor Decorate(this ServiceDescriptor descriptor, Type decoratorType)
        {
            return descriptor.WithFactory(
                provider => provider.CreateInstance(decoratorType, provider.GetInstance(descriptor)));
        }

        private static ServiceDescriptor WithFactory(
            this ServiceDescriptor descriptor,
            Func<IServiceProvider, object> factory)
        {
            return ServiceDescriptor.Describe(descriptor.ServiceType, factory, descriptor.Lifetime);
        }

        private static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationType != null)
            {
                return provider.GetServiceOrCreateInstance(descriptor.ImplementationType);
            }

            return descriptor.ImplementationFactory(provider);
        }

        private static object GetServiceOrCreateInstance(this IServiceProvider provider, Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(provider, type);
        }

        private static object CreateInstance(this IServiceProvider provider, Type type, params object[] arguments)
        {
            return ActivatorUtilities.CreateInstance(provider, type, arguments);
        }
    }
}

