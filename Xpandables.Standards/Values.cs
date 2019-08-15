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
    /// Defines a representation for an old and a new value of a specific type.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    [Serializable]
    [DebuggerDisplay("{OldValue}, {NewValue}")]
    public sealed class Values<TValue> : IFluent
    {
        /// <summary>
        /// Returns a new instance of <see cref="Values{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        public Values(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Contains the old value.
        /// </summary>
        public TValue OldValue { get; }

        /// <summary>
        /// Contains the new value.
        /// </summary>
        public TValue NewValue { get; }
    }
}