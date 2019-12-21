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

using System.Collections;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Describes an object that can contain a value or not of a specific type.
    /// You can make unconditional calls to its contents without testing whether the content is there or not.
    /// It can also contain an exception if available. The enumerator will only return the available value.
    /// It provides implicit operator for the expected <typeparamref name="T"/> value and <see cref="System.Exception"/>.
    /// If <typeparamref name="T"/> is an enumerable, use the <see cref="GetEnumerable"/> function to access its contain.
    /// </summary>
    /// <typeparam name="T">The Type of the value.</typeparam>
    public sealed partial class Optional<T> : IEnumerable<T>
    {
        private readonly Type[] _genericType = typeof(T).IsEnumerable() ? typeof(T).GetGenericArguments() : Type.EmptyTypes;
        private readonly T[] _values;
        private readonly Exception[] _exceptions;

        /// <summary>
        /// Gets the underlying value if exists.
        /// </summary>
        internal T InternalValue => _values[0];

        /// <summary>
        /// Gets the underlying exception if exists.
        /// </summary>
        private Exception InternalException => _exceptions[0];

        /// <summary>
        /// Determines whether the instance contains a value or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        internal bool IsValue() => _values.Length > 0;

        /// <summary>
        /// Determines whether the instance contains an exception or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        private bool IsException() => _exceptions.Length > 0;

        /// <summary>
        /// Determines whether the instance is empty (no value no exception) or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        private bool IsEmpty() => !IsValue() && !IsException();

        /// <summary>
        /// Returns an enumerator that iterates through the values.
        /// Do not use when <typeparamref name="T"/> is an enumerable, see <see cref="GetEnumerable"/>,
        /// otherwise it'll throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the values.</returns>
        /// <exception cref="InvalidOperationException">The <typeparamref name="T"/> is an enumerable, use <see cref="GetEnumerable"/>.
        /// </exception>
        public IEnumerator<T> GetEnumerator()
        {
            if (typeof(T).IsEnumerable())
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.GetEnumeratorInvalidOperation.StringFormat(typeof(T).Name, nameof(GetEnumerable)));
            }

            return ((IEnumerable<T>)_values).GetEnumerator();
        }

        /// <summary>
        /// Returns an System.Collections.IEnumerator for the System.Array.
        /// Do not use when <typeparamref name="T"/> is an enumerable, see <see cref="GetEnumerable"/>,
        /// otherwise it'll throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator for the System.Array.</returns>
        /// <exception cref="InvalidOperationException">The <typeparamref name="T"/> is an enumerable, use <see cref="GetEnumerable"/>.
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (typeof(T).IsEnumerable())
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.GetEnumeratorInvalidOperation.StringFormat(typeof(T).Name, nameof(GetEnumerable)));
            }

            return _values.GetEnumerator();
        }

        /// <summary>
        /// Returns the available enumerable value when <typeparamref name="T"/> is an enumerable.
        /// If enumerable value is null, it'll return an empty enumerable.
        /// Otherwise, its will throw an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <typeparamref name="T"/> is not an enumerable.</exception>
        public T GetEnumerable()
        {
            if (!typeof(T).IsEnumerable())
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.GetEnumerableInvalidOperation.StringFormat(typeof(T).Name));
            }

            return IsValue() ? _values[0] : GetDefaultEnumerable();
        }

        /// <summary>
        /// Get the stored value, or the default value for it's type.
        /// if the type is not null-able, it'll throw an exception.
        /// </summary>
        public T GetValueOrDefault()
            => IsValue()
                ? _values[0]
                : typeof(T).IsEnumerable()
                    ? GetDefaultEnumerable()
                    : default;

        /// <summary>
        /// Get the stored value, or return the provided default value
        /// Be aware if the type is not null-able.
        /// </summary>
        /// <param name="default">the value to be returned in replacement.</param>
        public T GetValueOrDefault(T @default)
            => IsValue()
                ? _values[0]
                : @default;

        /// <summary>
        /// Get the stored value, or return the provided default value.
        /// </summary>
        /// <param name="default">The delegate that provides with the replacement value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="default"/> is null.</exception>
        public T GetValueOrDefault(Func<T> @default)
        {
            if (@default is null) throw new ArgumentNullException(nameof(@default));
            return IsValue() ? _values[0] : @default();
        }

        /// <summary>
        /// Returns an empty T enumerable.
        /// </summary>
        private T GetDefaultEnumerable()
        {
            var runtimeMethod = OptionalBuilder.ArrayEmpty.MakeGenericMethod(_genericType[0]);
            return (T)runtimeMethod.Invoke(null, null);
        }

        internal Optional(Exception[] exceptions)
        {
            _values = Array.Empty<T>();
            _exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));
        }

        internal Optional(T[] values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _exceptions = Array.Empty<Exception>();
        }

        internal Optional()
        {
            _values = Array.Empty<T>();
            _exceptions = Array.Empty<Exception>();
        }
    }
}