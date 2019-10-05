﻿/************************************************************************************************************
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    internal sealed class LifetimeSelector : ILifetimeSelector, ISelector
    {
        public LifetimeSelector(ServiceTypeSelector inner, IEnumerable<TypeMap> typeMaps, IEnumerable<TypeFactoryMap> typeFactoryMaps)
        {
            Inner = inner;
            TypeMaps = typeMaps;
            TypeFactoryMaps = typeFactoryMaps;
        }

        private ServiceTypeSelector Inner { get; }

        private IEnumerable<TypeMap> TypeMaps { get; }

        private IEnumerable<TypeFactoryMap> TypeFactoryMaps { get; }

        public ServiceLifetime? Lifetime { get; set; }

        public IImplementationTypeSelector WithSingletonLifetime()
        {
            return WithLifetime(ServiceLifetime.Singleton);
        }

        public IImplementationTypeSelector WithScopedLifetime()
        {
            return WithLifetime(ServiceLifetime.Scoped);
        }

        public IImplementationTypeSelector WithTransientLifetime()
        {
            return WithLifetime(ServiceLifetime.Transient);
        }

        public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime)
        {
            Inner.PropagateLifetime(lifetime);

            return this;
        }

        #region Chain Methods

        public IImplementationTypeSelector FromCallingAssembly()
        {
            return Inner.FromCallingAssembly();
        }

        public IImplementationTypeSelector FromExecutingAssembly()
        {
            return Inner.FromExecutingAssembly();
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return Inner.FromEntryAssembly();
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return Inner.FromApplicationDependencies();
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
        {
            return Inner.FromApplicationDependencies(predicate);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
        {
            return Inner.FromAssemblyDependencies(assembly);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            return Inner.FromDependencyContext(context);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
        {
            return Inner.FromDependencyContext(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return Inner.FromAssemblyOf<T>();
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            return Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            return Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            return Inner.FromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return Inner.FromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddClasses()
        {
            return Inner.AddClasses();
        }

        public IServiceTypeSelector AddClasses(bool publicOnly)
        {
            return Inner.AddClasses(publicOnly);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
        {
            return Inner.AddClasses(action);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
        {
            return Inner.AddClasses(action, publicOnly);
        }

        public ILifetimeSelector AsSelf()
        {
            return Inner.AsSelf();
        }

        public ILifetimeSelector As<T>()
        {
            return Inner.As<T>();
        }

        public ILifetimeSelector As(params Type[] types)
        {
            return Inner.As(types);
        }

        public ILifetimeSelector As(IEnumerable<Type> types)
        {
            return Inner.As(types);
        }

        public ILifetimeSelector AsImplementedInterfaces()
        {
            return Inner.AsImplementedInterfaces();
        }

        public ILifetimeSelector AsSelfWithInterfaces()
        {
            return Inner.AsSelfWithInterfaces();
        }

        public ILifetimeSelector AsMatchingInterface()
        {
            return Inner.AsMatchingInterface();
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action)
        {
            return Inner.AsMatchingInterface(action);
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
        {
            return Inner.As(selector);
        }

        public IImplementationTypeSelector UsingAttributes()
        {
            return Inner.UsingAttributes();
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
        {
            return Inner.UsingRegistrationStrategy(registrationStrategy);
        }

        #endregion

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy strategy)
        {
            if (!Lifetime.HasValue)
            {
                Lifetime = ServiceLifetime.Transient;
            }

            strategy ??= RegistrationStrategy.Append;

            foreach (var typeMap in TypeMaps)
            {
                foreach (var serviceType in typeMap.ServiceTypes)
                {
                    var implementationType = typeMap.ImplementationType;

                    if (!implementationType.IsAssignableTo(serviceType))
                    {
                        throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""${serviceType.ToFriendlyName()}"".");
                    }

                    var descriptor = new ServiceDescriptor(serviceType, implementationType, Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }

            foreach (var typeFactoryMap in TypeFactoryMaps)
            {
                foreach (var serviceType in typeFactoryMap.ServiceTypes)
                {
                    var descriptor = new ServiceDescriptor(serviceType, typeFactoryMap.ImplementationFactory, Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }
        }
    }
}
