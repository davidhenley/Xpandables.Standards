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
    /// Defines a representation of an interval of a specific type.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    [Serializable]
    [DebuggerDisplay("Starting = {Starting}, Ending = {Ending}")]
    public struct IntervalValues<T> : IFluent, IEquatable<IntervalValues<T>>
        where T : struct
    {

        /// <summary>
        /// Returns a new instance of <see cref="IntervalValues{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="starting">The starting value</param>
        /// <param name="ending">The ending value</param>
        public IntervalValues(T starting, T ending)
        {
            Starting = starting;
            Ending = ending;
        }

        /// <summary>
        /// Provides with deconstruction for <see cref="IntervalValues{T}"/>.
        /// </summary>
        /// <param name="starting">The output starting value</param>
        /// <param name="ending">The output ending value</param>
        public void Deconstruct(out T starting, out T ending)
        {
            starting = Starting;
            ending = Ending;
        }

        /// <summary>
        /// Contains the starting value.
        /// </summary>
        public readonly T Starting { get; }

        /// <summary>
        /// Contains the ending value.
        /// </summary>
        public readonly T Ending { get; }

        /// <summary>
        /// Compares the <see cref="IntervalValues{T}"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object obj) => obj is IntervalValues<T> interval && Equals(interval);

        /// <summary>
        /// Computes the hash-code for the <see cref="IntervalValues{T}"/> instance.
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 17;
            hash += Starting.GetHashCode() ^ 31;
            hash += Ending.GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(IntervalValues<T> left, IntervalValues<T> right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(IntervalValues<T> left, IntervalValues<T> right) => !(left == right);

        /// <summary>
        /// Compares <see cref="IntervalValues{T}"/> with the value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(IntervalValues<T> other) => Starting.Equals(other.Starting) && Ending.Equals(other.Ending);

        /// <summary>
        /// Creates a string representation of the <see cref="IntervalValues{T}"/>.
        /// </summary>
        public readonly override string ToString() => $"{Starting}:{Ending}";

        /// <summary>
        /// Creates a string representation of the <see cref="IntervalValues{T}"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        public string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Starting, Ending);
    }
}