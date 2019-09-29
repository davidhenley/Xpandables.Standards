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

using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Defines a representation of an encrypted value with its key.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Key = {Key}, Value = {Value}")]
    public struct EncryptedValues : IFluent, IEquatable<EncryptedValues>
    {
        /// <summary>
        /// Returns a new instance of <see cref="EncryptedValues"/> with the key and value.
        /// </summary>
        /// <param name="key">The encrypted key.</param>
        /// <param name="value">The encrypted value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public EncryptedValues(string key, string value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Provides with deconstruction for <see cref="EncryptedValues"/>.
        /// </summary>
        /// <param name="key">The output key.</param>
        /// <param name="value">The output value.</param>
        public void Deconstruct(out string key, out string value)
        {
            key = Key;
            value = Value;
        }

        /// <summary>
        /// Contains the encryption key.
        /// </summary>
        [field: NonSerialized]
        public readonly string Key { get; }

        /// <summary>
        /// Contains the encrypted value.
        /// </summary>
        [field: NonSerialized]
        public readonly string Value { get; }

        /// <summary>
        /// Compares the <see cref="EncryptedValues"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object obj) => obj is EncryptedValues encryptedValues && Equals(encryptedValues);

        /// <summary>
        /// Computes the hash-code for the <see cref="EncryptedValues"/> instance.
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 17;
            hash += Key.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ 31;
            hash += Value.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(EncryptedValues left, EncryptedValues right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(EncryptedValues left, EncryptedValues right) => !(left == right);

        /// <summary>
        /// Compares <see cref="EncryptedValues"/> with the value.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(EncryptedValues other)
            => Key.Equals(other.Key, StringComparison.InvariantCultureIgnoreCase)
                && Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Creates a string representation of the <see cref="EncryptedValues"/>.
        /// </summary>
        public readonly override string ToString() => $"{Key}:{Value}";

        /// <summary>
        /// Creates a string representation of the <see cref="SignedValues{T}"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        public string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Key, Value);

    }
}