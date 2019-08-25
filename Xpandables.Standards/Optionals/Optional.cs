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
    /// Describes an object that contains a value or not of a specific type.
    /// You can make unconditional calls to its contents without testing whether the content is there or not.
    /// </summary>
    /// <typeparam name="T">The Type of the value.</typeparam>
#pragma warning disable CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés
#pragma warning disable CA1710 // Les identificateurs doivent avoir un suffixe correct
    public partial class Optional<T> : IEnumerable<T>
#pragma warning restore CA1710 // Les identificateurs doivent avoir un suffixe correct
#pragma warning restore CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés
    {
        private readonly T[] _values;

#pragma warning disable CA1000 // Ne pas déclarer de membres comme étant static sur les types génériques
        /// <summary>
        /// Provides with an optional without value.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public static Optional<T> Empty => new Optional<T>(Array.Empty<T>());

        /// <summary>
        /// Provides with an optional that contains a value.
        /// </summary>
        /// <param name="value">The value to be used for optional.</param>
        /// <returns>An optional with a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static Optional<T> Some(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return new Optional<T>(new T[] { value });
        }
#pragma warning restore CA1000 // Ne pas déclarer de membres comme étant static sur les types génériques

        /// <summary>
        /// Returns an enumerator that iterates through the values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the values.</returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

        /// <summary>
        /// Returns an System.Collections.IEnumerator for the System.Array.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator for the System.Array.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        protected Optional(T[] values) => _values = values ?? throw new ArgumentNullException(nameof(values));
    }
}