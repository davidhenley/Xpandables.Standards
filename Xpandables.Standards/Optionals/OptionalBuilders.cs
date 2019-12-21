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

using System.Reflection;

namespace System
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for building optional objects.
    /// </summary>
    public static class OptionalBuilder
    {
        /// <summary>
        /// Contains the empty method or the <see cref="Array"/> type used for building empty enumerable.
        /// </summary>
        public static readonly MethodInfo ArrayEmpty = typeof(Array).GetMethod("Empty");

        /// <summary>
        /// Converts the specified value to an optional instance.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic <see cref="Optional{T}"/>.</typeparam>
        /// <param name="value">The value to be stored.</param>
        /// <returns>A some <see cref="Optional{T}"/> whose type argument is <typeparamref name="T"/>.</returns>
        public static Optional<T> AsOptional<T>(this T value) => value is null ? Empty<T>() : Some(value);

        /// <summary>
        /// Converts the specified value to an optional instance that contains an exception.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic <see cref="Optional{T}"/>.</typeparam>
        /// <param name="exception">The exception to be stored.</param>
        /// <returns>An empty exception <see cref="Optional{T}"/> whose type argument is <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static Optional<T> AsOptional<T>(this Exception exception) => Exception<T>(exception);

        /// <summary>
        /// Converts the specified value to an optional instance of the specific type.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<T> AsOptional<T>(this object value) => value is T target ? target.AsOptional() : Empty<T>();

        /// <summary>
        /// Converts the specified value to an optional pair instance.
        /// if one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <typeparam name="TU">The type of the right value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <param name="right">The right value to act on.</param>
        /// <returns>An optional pair instance.</returns>
        public static Optional<(T Left, TU Right)> AsOptional<T, TU>(this T value, TU right) => value.AsOptional().And(right);

        /// <summary>
        /// Converts the specified optional to an optional pair instance.
        /// if one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <typeparam name="TU">The type of the right value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="right">The right value to act on.</param>
        /// <returns>An optional pair instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        public static Optional<(T Left, TU Right)> AsOptional<T, TU>(this Optional<T> optional, TU right)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            return optional.And(right);
        }

        /// <summary>
        /// Returns an empty <see cref="Optional{T}"/> that has the specified type argument.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic <see cref="Optional{T}"/>.</typeparam>
        /// <returns>An empty <see cref="Optional{T}"/> whose type argument is <typeparamref name="T"/>.</returns>
        public static Optional<T> Empty<T>() => new Optional<T>();

        /// <summary>
        /// Returns a some <see cref="Optional{T}"/> that has the specified type argument and value.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic <see cref="Optional{T}"/>.</typeparam>
        /// <param name="value">The value to be stored.</param>
        /// <returns>A some <see cref="Optional{T}"/> whose type argument is <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static Optional<T> Some<T>(T value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            return new Optional<T>(new T[] { value });
        }

        /// <summary>
        /// Returns an empty exception <see cref="Optional{T}"/> that has the specified type argument and exception.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic <see cref="Optional{T}"/>.</typeparam>
        /// <param name="exception">The exception to be stored.</param>
        /// <returns>An empty exception <see cref="Optional{T}"/> whose type argument is <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static Optional<T> Exception<T>(Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));
            return new Optional<T>(new Exception[] { exception });
        }
    }
}
