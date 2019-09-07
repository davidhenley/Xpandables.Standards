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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    internal class TypeSourceSelector : ITypeSourceSelector, ISelector
    {
        private List<ISelector> Selectors { get; } = new List<ISelector>();

        /// <inheritdoc />
        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return InternalFromAssembliesOf(new[] { typeof(T).GetTypeInfo() });
        }

        public IImplementationTypeSelector FromCallingAssembly()
        {
            return FromAssemblies(Assembly.GetCallingAssembly());
        }

        public IImplementationTypeSelector FromExecutingAssembly()
        {
            return FromAssemblies(Assembly.GetExecutingAssembly());
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return FromAssemblies(Assembly.GetEntryAssembly());
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return FromApplicationDependencies(_ => true);
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
        {
            try
            {
                return FromDependencyContext(DependencyContext.Default, predicate);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // Something went wrong when loading the DependencyContext, fall
                // back to loading all referenced assemblies of the entry assembly...
                return FromAssemblyDependencies(Assembly.GetEntryAssembly());
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            return FromDependencyContext(context, _ => true);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            var assemblies = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Select(Assembly.Load)
                .Where(predicate)
                .ToArray();

            return InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
        {
            if (assembly is null) throw new ArgumentNullException(nameof(assembly));

            var assemblies = new List<Assembly> { assembly };

            try
            {
                var dependencyNames = assembly.GetReferencedAssemblies();

                foreach (var dependencyName in dependencyNames)
                {
                    try
                    {
                        // Try to load the referenced assembly...
                        assemblies.Add(Assembly.Load(dependencyName));
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch
                    {
                        // Failed to load assembly. Skip it.
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }

                return InternalFromAssemblies(assemblies);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                return InternalFromAssemblies(assemblies);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            return InternalFromAssembliesOf(types.Select(x => x.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (!types.Any()) throw new ArgumentOutOfRangeException(nameof(types));

            return InternalFromAssembliesOf(types.Select(t => t.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.Length == 0) throw new ArgumentOutOfRangeException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
            if (!assemblies.Any()) throw new ArgumentOutOfRangeException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddTypes(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0) throw new ArgumentOutOfRangeException(nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector.AddClasses();
        }

        public IServiceTypeSelector AddTypes(IEnumerable<Type> types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            if (!types.Any()) throw new ArgumentOutOfRangeException(nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector.AddClasses();
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            foreach (var selector in Selectors)
            {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<TypeInfo> typeInfos)
        {
            return InternalFromAssemblies(typeInfos.Select(t => t.Assembly));
        }

        private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes).Select(x => x.AsType()));
        }

        private IImplementationTypeSelector AddSelector(IEnumerable<Type> types)
        {
            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector;
        }
    }
}
