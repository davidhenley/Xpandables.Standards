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
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Describes an encrypted value and its key.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Key}, {Value}")]
    public sealed class ValueEncrypted : ValueObject
    {
        private ValueEncrypted(string key, string value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValueEncrypted"/> with key and value.
        /// </summary>
        /// <param name="key">The encrypted key.</param>
        /// <param name="value">The encrypted value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static ValueEncrypted CreateWith(string key, string value) => new ValueEncrypted(key, value);

        /// <summary>
        /// provides with deconstruction for <see cref="ValueEncrypted"/>.
        /// </summary>
        /// <param name="key">The output key.</param>
        /// <param name="value">The output value.</param>
        public void Deconstruct(out string key, out string value)
        {
            key = Key;
            value = Value;
        }

        /// <summary>
        /// Gets the encryption key used for the encrypted value.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the encrypted value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Provide the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Key;
            yield return Value;
        }
    }
}