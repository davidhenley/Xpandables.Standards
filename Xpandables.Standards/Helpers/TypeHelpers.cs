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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Extensions methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeHelpers
    {
        /// <summary>
        /// Determines whether the specified method is overridden in its current implementation.
        /// </summary>
        /// <param name="methodInfo">The method info to act on.</param>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is null.</exception>
        public static bool IsOverridden(this MethodInfo methodInfo)
        {
            if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
            return methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;
        }

        /// <summary>
        /// Determines whether the specified method is a <see cref="Task"/>.
        /// </summary>
        /// <param name="methodInfo">The method info to act on.</param>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is null.</exception>
        public static bool IsAwaitable(this MethodInfo methodInfo)
        {
            if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
            return methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
        }

        /// <summary>
        /// Determines whether the current generic type is a nullable type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
        public static bool IsGenericNullable(this Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether the current type is a null-able type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
        public static bool IsNullable(this Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return !type.IsPrimitive && Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Determines whether the current type implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
        /// <exception cref="TargetInvocationException">A static initializer is invoked and throws an exception.</exception>
        public static bool IsEnumerable(this Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return !type.IsPrimitive
                    && type != typeof(string)
                    && type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// Returns the name of the type without the generic arity '`'.
        /// Useful for generic types.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The name of the type without the generic arity '`'.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static string GetNameWithoutGenericArity(this Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            var index = type.Name.IndexOf('`', StringComparison.InvariantCulture);
            return index == -1 ? type.Name : type.Name.Substring(0, index);
        }

        /// <summary>
        /// Creates an instance of the specified type with optional arguments.
        /// </summary>
        /// <param name="type">Type of the instance to be created.</param>
        /// <param name="args">Optional list of arguments to be passed to the constructor.
        /// Arguments must be specified as expected (order and type) by the constructor,
        /// otherwise an exception will be thrown.</param>
        /// <returns>A new instance of the specified type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            try
            {
                return args.Count(arg => arg == null) == args.Length
                        ? Activator.CreateInstance(type)
                        : Activator.CreateInstance(type, args);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Creating the type {type.Name} failed. See inner exception",
                    exception);
            }
        }

        /// <summary>
        /// Creates an instance of specified type with optional arguments.
        /// </summary>
        /// <typeparam name="T">Type to be instantiated.</typeparam>
        /// <param name="args">Optional list of arguments to be passed to the constructor.
        /// Arguments must be specified as expected (order and type) by the constructor,
        /// otherwise an exception will be thrown.</param>
        /// <returns>A new instance of the specified type.</returns>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static T CreateInstance<T>(params object[] args)
            where T : class => (T)CreateInstance(typeof(T), args);

        /// <summary>
        /// Returns the description attribute value of the specified property info.
        /// </summary>
        /// <param name="source">The type to act on.</param>
        /// <returns>A string description if found otherwise null.</returns>
        public static string GetDescription(this PropertyInfo source)
            => source
                ?.GetCustomAttributes(true)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault()
                ?.Description ?? string.Empty;
    }
}