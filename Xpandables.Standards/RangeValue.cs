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

using System.ComponentModel;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Defines a pair of values, representing a segment.
    /// This class uses <see cref="RangeValueConverter"/> as type converter.
    /// </summary>
    /// <typeparam name="T">The Type of each of two values of range.</typeparam>
    [Serializable]
    [DebuggerDisplay("Min = {Min}, Max = {Max}")]
    [TypeConverter(typeof(RangeValueConverter))]
    public struct RangeValue<T> : IFluent, IEquatable<RangeValue<T>>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RangeValue{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="min">The minimal value of range.</param>
        /// <param name="max">The maximal value of range.</param>
        public RangeValue(T min, T max) => (Min, Max) = (min, max);

        /// <summary>
        /// Provides with deconstruction for <see cref="RangeValue{T}"/>.
        /// </summary>
        /// <param name="min">The output minimal value of range.</param>
        /// <param name="max">The output maximal value of range.</param>
        public void Deconstruct(out T min, out T max) => (min, max) = (Min, Max);

        /// <summary>
        /// Gets the minimal value of range.
        /// </summary>
        public readonly T Min { get; }

        /// <summary>
        /// Gets the maximal value of range.
        /// </summary>
        public readonly T Max { get; }

        /// <summary>
        /// Compares the <see cref="RangeValue{T}"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object obj) => obj is RangeValue<T> signedValues && Equals(signedValues);

        /// <summary>
        /// Computes the hash-code for the <see cref="RangeValue{T}"/> instance.
        /// </summary>
        public readonly override int GetHashCode()
        {
            var hash = 17;
            hash += Min.GetHashCode() ^ 31;
            hash += Max.GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(RangeValue<T> left, RangeValue<T> right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(RangeValue<T> left, RangeValue<T> right) => !(left == right);

        /// <summary>
        /// Compares <see cref="RangeValue{T}"/> with the value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(RangeValue<T> other) => Min.Equals(other.Min) && Max.Equals(other.Max);

        /// <summary>
        /// Creates a string representation of the <see cref="RangeValue{T}"/> separated by ":".
        /// </summary>
        public readonly override string ToString() => $"{Min}:{Max}";

        /// <summary>
        /// Creates a string representation of the <see cref="RangeValue{T}"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public readonly string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Min, Max);

        /// <summary>
        /// Determines whether this range is empty or not.
        /// Returns <see langword="true"/> if so, otherwise returns <see langword="false"/>.
        /// </summary>
        public bool IsEmpty() => Min.CompareTo(Max) >= 0;
    }
}