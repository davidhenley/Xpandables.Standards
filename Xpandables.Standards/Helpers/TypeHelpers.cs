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
using System.Globalization;
using System.IO;
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
        /// Determines whether the current type is an open generic type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns><see langword="true"/> if found, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static bool IsOpenGeneric(this Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return type.GetTypeInfo().IsGenericTypeDefinition;
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

            var index = type.Name.IndexOf('`', StringComparison.OrdinalIgnoreCase);
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

        /// <summary>
        /// Returns a loaded assembly from its name.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>If found, returns a loaded assembly otherwise an empty result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName"/> is null.</exception>
        public static Optional<Assembly> AssemblyLoadedFromString(this string assemblyName)
        {
            if (assemblyName is null) throw new ArgumentNullException(nameof(assemblyName));

            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception exception) when (exception is ArgumentException
                                            || exception is FileNotFoundException
                                            || exception is FileLoadException
                                            || exception is BadImageFormatException)
            {
                return Optional<Assembly>.Exception(exception);
            }
        }

        /// <summary>
        /// Returns type from its string name.
        /// </summary>
        /// <param name="typeName">The name of the type to find.</param>
        /// <returns>if found, returns the type otherwise an empty result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> is null.</exception>
        public static Optional<Type> TypeFromString(this string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException(nameof(typeName));

            try
            {
                return Type.GetType(typeName, true, true);
            }
            catch (Exception exception) when (exception is TargetInvocationException
                                            || exception is TypeLoadException
                                            || exception is ArgumentException
                                            || exception is FileNotFoundException
                                            || exception is FileLoadException
                                            || exception is BadImageFormatException)
            {
                return Optional<Type>.Exception(exception);
            }
        }

        /// <summary>
        /// Returns the type, if not found, try to load from the assembly.
        /// </summary>
        /// <param name="typeName">The name of the type to find.</param>
        /// <param name="assemblyName">The assembly to act on.</param>
        /// <returns>if found, returns the type otherwise an empty result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName"/> is null.</exception>
        public static Optional<Type> TypeFromString(this string typeName, string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentNullException(nameof(assemblyName));
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException(nameof(typeName));

            var resultFromType = TypeFromString(typeName);
            if (resultFromType.Any()) return resultFromType;

            var resultFromAss = AssemblyLoadedFromString(assemblyName);
            if (resultFromAss.Any())
            {
                var resultLoadedType = resultFromAss.Single()
                    .GetExportedTypes()
                    .FirstOrEmpty(t => t.FullName.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));

                return resultLoadedType
                    .Map(value => value);
            }

            var exception = default(Exception);
            resultFromAss.WhenException(ex => exception = ex);

            return Optional<Type>.Exception(exception);
        }

        /// <summary>
        /// Invokes the specified member, using the specified binding constraints and matching
        /// the specified argument list.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName">The string containing the name of the constructor, method, property, or field
        /// member to invoke. /// -or- /// An empty string (&quot;&quot;) to invoke the default
        /// member. /// -or- /// For IDispatch members, a string representing the DispID,
        /// for example &quot;[DispID=3]&quot;.</param>
        /// <param name="invokeAttr">A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        /// how the search is conducted. The access can be one of the BindingFlags such as
        /// Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup
        /// need not be specified. If the type of lookup is omitted, BindingFlags.Public
        /// | BindingFlags.Instance | BindingFlags.Static are used.</param>
        /// <param name="binder">An object that defines a set of properties and enables binding, which can involve
        /// selection of an overloaded method, coercion of argument types, and invocation
        /// of a member through reflection. /// -or- /// A null reference (Nothing in Visual
        /// Basic), to use the System.Type.DefaultBinder. Note that explicitly defining a
        /// System.Reflection.Binder object may be required for successfully invoking method
        /// overloads with variable arguments.</param>
        /// <param name="target">The object on which to invoke the specified member.</param>
        /// <param name="args">An array containing the arguments to pass to the member to invoke.</param>
        /// <returns>An object representing the return value of the invoked member
        /// or an empty result with handled exception.</returns>
        public static Optional<object> TypeInvokeMember(
            this Type type,
            string memberName,
            BindingFlags invokeAttr,
             Binder binder,
            object target,
             object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentNullException(nameof(memberName));

            try
            {
                return type.InvokeMember(memberName, invokeAttr, binder, target, args, CultureInfo.InvariantCulture);
            }
            catch (Exception exception) when (exception is ArgumentNullException
                                            || exception is ArgumentException
                                            || exception is MethodAccessException
                                            || exception is MissingFieldException
                                            || exception is MissingMethodException
                                            || exception is TargetException
                                            || exception is AmbiguousMatchException
                                            || exception is InvalidOperationException)
            {
                return Optional<object>.Exception(exception);
            }
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the
        /// current generic type definition and returns a System.Type object representing
        /// the resulting constructed type. If error, return an optional with exception.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="typeArguments">An array of types to be substituted for the type parameters of the current generic
        /// type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static Optional<Type> MakeGenericTypeSafe(this Type type, params Type[] typeArguments)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try
            {
                return type.MakeGenericType(typeArguments);
            }
            catch (Exception exception) when (exception is InvalidOperationException
                                                || exception is ArgumentException
                                                || exception is NotSupportedException)
            {
                return Optional<Type>.Exception(exception);
            }
        }
    }
}