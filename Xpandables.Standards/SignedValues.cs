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
    [DebuggerDisplay("{Positive}, {Negative}")]
    public sealed class SignedValues<T> : IValidatableAttribute
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Returns a new instance of <see cref="SignedValues{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="positive">The positive value</param>
        /// <param name="negative">The negative value</param>
        public SignedValues(T positive, T negative)
        {
            Positive = positive;
            Negative = negative;
        }

        /// <summary>
        /// Contains the positive value.
        /// </summary>
        public T Positive { get; }

        /// <summary>
        /// Contains the negative value.
        /// </summary>
        public T Negative { get; }
    }
}