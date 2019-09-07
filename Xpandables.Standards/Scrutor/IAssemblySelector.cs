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

using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    public interface IAssemblySelector : IFluent
    {
        /// <summary>
        /// Will scan for types from the calling assembly.
        /// </summary>
        IImplementationTypeSelector FromCallingAssembly();

        /// <summary>
        /// Will scan for types from the currently executing assembly.
        /// </summary>
        IImplementationTypeSelector FromExecutingAssembly();

        /// <summary>
        /// Will scan for types from the entry assembly.
        /// </summary>
        IImplementationTypeSelector FromEntryAssembly();

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently executing application.
        /// Calling this method is equivalent to calling <see cref="FromDependencyContext(DependencyContext)"/> and passing in <see cref="DependencyContext.Default"/>.
        /// </summary>
        /// <remarks>
        /// If loading <see cref="DependencyContext.Default"/> fails, this method will fall back to calling <see cref="FromAssemblyDependencies(Assembly)"/>,
        /// using the entry assembly.
        /// </remarks>
        IImplementationTypeSelector FromApplicationDependencies();

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently executing application.
        /// Calling this method is equivalent to calling <see cref="FromDependencyContext(DependencyContext, Func{Assembly, bool})"/> and passing in <see cref="DependencyContext.Default"/>.
        /// </summary>
        /// <remarks>
        /// If loading <see cref="DependencyContext.Default"/> fails, this method will fall back to calling <see cref="FromAssemblyDependencies(Assembly)"/>,
        /// using the entry assembly.
        /// </remarks>
        /// <param name="predicate">The predicate to match assemblies.</param>
        IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate);

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly whose dependencies should be scanned.</param>
        IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly);

        /// <summary>
        /// Will load and scan all runtime libraries in the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The dependency context.</param>
        IImplementationTypeSelector FromDependencyContext(DependencyContext context);

        /// <summary>
        /// Will load and scan all runtime libraries in the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The dependency context.</param>
        /// <param name="predicate">The predicate to match assemblies.</param>
        IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate);

        /// <summary>
        /// Will scan for types from the assembly of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
        IImplementationTypeSelector FromAssemblyOf<T>();

        /// <summary>
        /// Will scan for types from the assemblies of each <see cref="Type"/> in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The types in which assemblies that should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(params Type[] types);

        /// <summary>
        /// Will scan for types from the assemblies of each <see cref="Type"/> in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The types in which assemblies that should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types);

        /// <summary>
        /// Will scan for types in each <see cref="Assembly"/> in <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies to should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="assemblies"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies);

        /// <summary>
        /// Will scan for types in each <see cref="Assembly"/> in <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies to should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="assemblies"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies);
    }
}
