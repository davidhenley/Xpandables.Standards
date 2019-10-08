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
using System.Linq;

namespace System
{
    /// <summary>
    /// An object that represents a descriptive aspect of the domain with no conceptual identity.
    /// <para><see cref="ValueObject"/> are instantiated to represent elements of the design that we care about only
    /// for what they are not who or which they are.” [Source : Evans 2003]</para>
    /// This is an <see langword="abstract"/> and <see langword="serializable"/> class.
    /// </summary>
    [Serializable]
    public abstract class ValueObject
    {
        /// <summary>
        /// When implemented in derived class, this method will provide the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// The comparison is done by using SequenceEqual() on the two sets of components.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
            => obj == null || obj.GetType() != GetType()
                ? false
                : GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return (current * 23) + (obj?.GetHashCode() ?? 0);
                    }
                });

        /// <summary>
        /// Compares equality.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
            => left is null ^ right is null ? false : left?.Equals(right) != false;

        /// <summary>
        /// Compares equality.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public static bool operator ==(ValueObject left, ValueObject right) => EqualOperator(left, right);

        /// <summary>
        /// Compares difference.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>true if the specified objects are not equal; otherwise, false.</returns>
        public static bool operator !=(ValueObject left, ValueObject right) => NotEqualOperator(left, right);

        /// <summary>
        /// Compares difference.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>true if the specified objects are not equal; otherwise, false.</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right) => !EqualOperator(left, right);
    }
}