﻿/************************************************************************************************************
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
    /// <typeparam name="T">Type of the value.</typeparam>
    [Serializable]
    [DebuggerDisplay("{OldValue}, {NewValue}")]
    public sealed class Values<T> : IFluent
    {
        /// <summary>
        /// Returns a new instance of <see cref="Values{T}"/> with the specified values.
        /// </summary>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        public Values(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Provides with deconstruction for <see cref="Values{T}"/>.
        /// </summary>
        /// <param name="oldValue">The output old value.</param>
        /// <param name="newValue">The output new value.</param>
        public void Deconstruct(out T oldValue, out T newValue)
        {
            oldValue = OldValue;
            newValue = NewValue;
        }

        /// <summary>
        /// Contains the old value.
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// Contains the new value.
        /// </summary>
        public T NewValue { get; }
    }
}