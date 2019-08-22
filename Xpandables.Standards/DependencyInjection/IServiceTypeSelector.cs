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

using System.Collections.Generic;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    public interface IServiceTypeSelector : IImplementationTypeSelector
    {
        /// <summary>
        /// Registers each matching concrete type as itself.
        /// </summary>
        ILifetimeSelector AsSelf();

#pragma warning disable CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés
        /// <summary>
        /// Registers each matching concrete type as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to register as.</typeparam>
        ILifetimeSelector As<T>();

        /// <summary>
        /// Registers each matching concrete type as each of the specified <paramref name="types" />.
        /// </summary>
        /// <param name="types">The types to register as.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(params Type[] types);

        /// <summary>
        /// Registers each matching concrete type as each of the specified <paramref name="types" />.
        /// </summary>
        /// <param name="types">The types to register as.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(IEnumerable<Type> types);

        /// <summary>
        /// Registers each matching concrete type as all of its implemented interfaces.
        /// </summary>
        ILifetimeSelector AsImplementedInterfaces();
#pragma warning restore CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés

        /// <summary>
        /// Registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type
        /// </summary>
        ILifetimeSelector AsSelfWithInterfaces();

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName)
        /// </summary>
        ILifetimeSelector AsMatchingInterface();

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName)
        /// </summary>
        /// <param name="action">Filter for matching the Type to an implementing interface</param>
        ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action);

#pragma warning disable CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés
        /// <summary>
        /// Registers each matching concrete type as each of the types returned
        /// from the <paramref name="selector"/> function.
        /// </summary>
        /// <param name="selector">A function to select service types based on implementation types.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="selector"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector);
#pragma warning restore CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés

        /// <summary>
        /// Registers each matching concrete type according to their <see cref="ServiceDescriptorAttribute"/>.
        /// </summary>
        IImplementationTypeSelector UsingAttributes();

        IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy);
    }
}
