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

namespace System
{
#nullable disable
    public partial class Optional<T>
        : IEquatable<Optional<T>>, IEquatable<T>, IComparable<Optional<T>>, IComparable<T>, IFormattable
    {
        /// <summary>
        /// Compares the order of two optionals.
        /// </summary>
        /// <param name="other">The other optional to compare with.</param>
        /// <returns>An <see cref="int"/> that indicates the order of the optionals being compared.</returns>
        public int CompareTo(Optional<T> other)
        {
            if (other is null) return 1;
            if (!IsEmpty() && other.IsEmpty()) return 1;
            if (IsValue() && other.IsValue())
                return Comparer<T>.Default.Compare(InternalValue, other.InternalValue);
            if (IsException() && other.IsException())
                return Comparer<Exception>.Default.Compare(InternalException, other.InternalException);
            if (IsValue() && other.IsException()) return 1;
            if (IsException() && other.IsValue())
                return -1;
            return 0;
        }

        public int CompareTo(T other)
        {
            if (IsValue() && EqualityComparer<T>.Default.Equals(other, default)) return 1;
            return !IsValue() && !EqualityComparer<T>.Default.Equals(other, default)
                ? -1
                : Comparer<T>.Default.Compare(InternalValue, other);
        }

        /// <summary>
        /// Compares two instances of the <see cref="Optional{T}"/>.
        /// Two options are equal if both are of the same type, the same case
        /// and the underlying values are equal.
        /// </summary>
        /// <param name="other">Option to compare with</param>
        public bool Equals(Optional<T> other)
        {
            if (other == null) return false;
            if (IsEmpty() && other.IsEmpty()) return true;
            if (IsValue() && other.IsValue())
                return EqualityComparer<T>.Default.Equals(InternalValue, other.InternalValue);
            if (IsException() && other.IsException())
                return EqualityComparer<Exception>.Default.Equals(InternalException, other.InternalException);
            return false;
        }

        /// <summary>
        /// Compares <see cref="Optional{T}"/> with the value of type <typeparamref name="T"/>.
        /// Option is equal to the value of the underlying type if it's Some case
        /// and encapsulated value is equal to <paramref name="other"/> value.
        /// </summary>
        /// <param name="other">Option to compare with</param>
        public bool Equals(T other)
            => IsValue() && EqualityComparer<T>.Default.Equals(InternalValue, other);

        /// <summary>
        /// Compares the <see cref="Optional{T}"/> with other object.
        /// Option is only equal to other option given the conditions described in <see cref="Optional{T}.Equals(Optional{T})"/>.
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        public override bool Equals(object obj) => obj is Optional<T> iOptional && Equals(iOptional);

        /// <summary>
        /// Computes the hash-code for the <see cref="Optional{T}"/> instance.
        /// </summary>
        public override int GetHashCode()
        {
            const int hash = 17;
            if (IsValue())
                return InternalValue.GetHashCode() ^ 31;
            if (IsException())
                return InternalException.GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Creates a string representation of the <see cref="Optional{T}"/>.
        /// </summary>
        public override string ToString()
            => IsValue() ? $"{InternalValue}" : IsException() ? $"{InternalException}" : string.Empty;

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => IsValue()
                ? string.Format(formatProvider, "{0:" + format + "}", InternalValue)
                : IsException()
                    ? string.Format(formatProvider, "{0:" + format + "}", InternalException)
                    : string.Empty;
    }
#nullable enable
}