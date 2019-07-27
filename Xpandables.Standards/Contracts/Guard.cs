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

namespace System
{
    /// <summary>
    /// Defines a class that contains a non-null value of a specific type.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public struct Guard<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Guard{T}"/> with its value.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        internal Guard(T value) => Value = value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// Gets the value of the <typeparamref name="T"/> type.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Implicit returns the value matching the expected type.
        /// </summary>
        /// <param name="guard"></param>
        public static implicit operator T(Guard<T> guard) => guard.Value;

        /// <summary>
        /// Implicit returns an instance of <see cref="Guard{T}"/> from the specified value.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        public static implicit operator Guard<T>(T value) => Guard.Create(value);

        /// <summary>
        /// Returns the comparison value of both <see cref="Guard{T}"/> objects.
        /// </summary>
        /// <param name="obj">The other object for comparison.</param>
        /// <returns>A boolean value.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Guard<T> other))
                return false;

            return GetType() == other.GetType()
                && Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns the hash-code of the current type.
        /// </summary>
        /// <returns>hash-code.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Compares equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Guard<T> left, Guard<T> right) => left.Value.Equals(right.Value);

        /// <summary>
        ///Compares difference.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Guard<T> left, Guard<T> right) => left.Value != right.Value;
    }
}