/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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
using System.Linq;

namespace System
{
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
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
            if (this.Any() && !other.Any()) return 1;
            if (!this.Any() && other.Any()) return -1;

            return Comparer<T>.Default.Compare(this.Single(), other.Single());
        }

        public int CompareTo(T other)
        {
            if (this.Any() && other is null) return 1;
            return !this.Any() && !(other is null) ? -1 : Comparer<T>.Default.Compare(this.Single(), other);
        }

        /// <summary>
        /// Compares two instances of the <see cref="IOptional"/>.
        /// Two options are equal if both are of the same type, the same case
        /// and the underlying values are equal.
        /// </summary>
        /// <param name="other">Option to compare with</param>
        public bool Equals(Optional<T> other) =>
            (!this.Any() && !other.Any()) || (this.Any() && other.Any()
            && EqualityComparer<T>.Default.Equals(this.Single(), other.Single()));

        /// <summary>
        /// Compares <see cref="IOptional"/> with the value of type <typeparamref name="T"/>.
        /// Option is equal to the value of the underlying type if it's Some case
        /// and encapsulated value is equal to <paramref name="other"/> value.
        /// </summary>
        /// <param name="other">Option to compare with</param>
        public bool Equals(T other) => this.Any() && EqualityComparer<T>.Default.Equals(this.Single(), other);

        /// <summary>
        /// Compares the <see cref="IOptional"/> with other object.
        /// Option is only equal to other option given the conditions described in <see cref="Optional{T}.Equals(Optional{T})"/>.
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        public override bool Equals(object obj) => obj is Optional<T> iOptional && Equals(iOptional);

        /// <summary>
        /// Computes the hash-code for the <see cref="Optional{T}"/> instance.
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 17;
            if (this.Any())
                return this.Single().GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Creates a string representation of the <see cref="Optional{T}"/>.
        /// </summary>
        public override string ToString() => this.Any() ? $"{this.Single()}" : string.Empty;

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => this.Any()
                ? string.Format(formatProvider, "{0:" + format + "}", this.Single())
                : string.Empty;
    }
}