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
    /// Defines a representation for a signed value : positive and negative form.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    [Serializable]
    [DebuggerDisplay("Positive = {Positive}, Negative = {Negative}")]
    public struct SignedValues<T> : IFluent, IEquatable<SignedValues<T>>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Returns a new instance of <see cref="SignedValues{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="positive">The positive value</param>
        /// <param name="negative">The negative value</param>
        public SignedValues(T positive, T negative) => (Positive, Negative) = (positive, negative);

        /// <summary>
        /// Provides with deconstruction for <see cref="SignedValues{T}"/>.
        /// </summary>
        /// <param name="positive">The output positive value.</param>
        /// <param name="negative">The output negative value.</param>
        public void Deconstruct(out T positive, out T negative) => (positive, negative) = (Positive, Negative);

        /// <summary>
        /// Contains the positive value.
        /// </summary>
        public readonly T Positive { get; }

        /// <summary>
        /// Contains the negative value.
        /// </summary>
        public readonly T Negative { get; }

        /// <summary>
        /// Compares the <see cref="SignedValues{T}"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object obj) => obj is SignedValues<T> signedValues && Equals(signedValues);

        /// <summary>
        /// Computes the hash-code for the <see cref="SignedValues{T}"/> instance.
        /// </summary>
        public readonly override int GetHashCode()
        {
            var hash = 17;
            hash += Positive.GetHashCode() ^ 31;
            hash += Negative.GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(SignedValues<T> left, SignedValues<T> right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(SignedValues<T> left, SignedValues<T> right) => !(left == right);

        /// <summary>
        /// Compares <see cref="SignedValues{T}"/> with the value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(SignedValues<T> other) => Positive.Equals(other.Positive) && Negative.Equals(other.Negative);

        /// <summary>
        /// Creates a string representation of the <see cref="SignedValues{T}"/>.
        /// </summary>
        public readonly override string ToString() => $"{Positive}:{Negative}";

        /// <summary>
        /// Creates a string representation of the <see cref="SignedValues{T}"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public readonly string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Positive, Negative);
    }
}