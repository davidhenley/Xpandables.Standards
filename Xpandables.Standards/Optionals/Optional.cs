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
using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Describes an object that contains a value or not of a specific type.
    /// You can make unconditional calls to its contents without testing whether the content is there or not.
    /// It can also contain an exception if available. The enumerator will only return the available value.
    /// It provides implicit operator for the expected <typeparamref name="T"/> value and <see cref="System.Exception"/>.
    /// </summary>
    /// <typeparam name="T">The Type of the value.</typeparam>
    public sealed partial class Optional<T> : IEnumerable<T>
    {
        private readonly T[] Values;
        private readonly Exception[] Exceptions;

        /// <summary>
        /// Gets the underlying value if exists.
        /// </summary>
        internal T InternalValue => Values[0];

        /// <summary>
        /// Gets the underlying exception if exists.
        /// </summary>
        internal Exception InternalException => Exceptions[0];

        /// <summary>
        /// Determines whether the instance contains a value or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        internal bool IsValue() => Values.Length > 0;

        /// <summary>
        /// Determines whether the instance contains an exception or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        internal bool IsException() => Exceptions.Length > 0;

        /// <summary>
        /// Determines whether the instance is empty (no value no exception) or not.
        /// If so, returns <see langword="true"/>, otherwise returns <see langword="false"/>.
        /// </summary>
        internal bool IsEmpty() => !IsValue() && !IsException();

        /// <summary>
        /// Provides with an optional of the specific type that is empty.
        /// </summary>
        /// <returns>An optional with no value nor exception.</returns>
        [SuppressMessage("Design", "CA1000:Ne pas déclarer de membres comme étant static sur les types génériques",
            Justification = "<En attente>")]
        public static Optional<T> Empty() => new Optional<T>();

        /// <summary>
        /// Provides with an optional that contains a value of specific type.
        /// </summary>
        /// <param name="value">The value to be used for optional.</param>
        /// <returns>An optional with a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        [SuppressMessage("Design", "CA1000:Ne pas déclarer de membres comme étant static sur les types génériques",
            Justification = "<En attente>")]
        public static Optional<T> Some([NotNull] T value)
        {
#nullable disable
            if (EqualityComparer<T>.Default.Equals(value, default)) throw new ArgumentNullException(nameof(value));
#nullable enable
            return new Optional<T>(new T[] { value });
        }

        /// <summary>
        /// Provides with an optional of specific type that contains the specified exception.
        /// </summary>
        /// <param name="exception">The exception to store.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        [SuppressMessage("Design", "CA1000:Ne pas déclarer de membres comme étant static sur les types génériques",
            Justification = "<En attente>")]
        public static Optional<T> Exception([NotNull] Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));
            return new Optional<T>(new Exception[] { exception });
        }

        /// <summary>
        /// Returns an enumerator that iterates through the values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the values.</returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Values).GetEnumerator();

        /// <summary>
        /// Returns an System.Collections.IEnumerator for the System.Array.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator for the System.Array.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();

        private Optional(Exception[] exceptions)
        {
            Values = Array.Empty<T>();
            Exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));
        }

        private Optional(T[] values)
        {
            Values = values ?? throw new ArgumentNullException(nameof(values));
            Exceptions = Array.Empty<Exception>();
        }

        private Optional()
        {
            Values = Array.Empty<T>();
            Exceptions = Array.Empty<Exception>();
        }
    }
}