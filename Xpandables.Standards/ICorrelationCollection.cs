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
    /// <summary>
    /// Provides a collection of objects that need to be shared across asynchronous control flows.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface ICorrelationCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Sets the correlated object by its key. If key already exist, it'll be updated.
        /// Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A object from the ambient correlation list of objects.</returns>
        Optional<TValue> this[TKey key] { set; get; }

        /// <summary>
        /// Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A object from the ambient correlation list of objects.</returns>
        Optional<TValue> GetValue(TKey key);

        /// <summary>
        /// Adds the correlated object by its key. If key already exist, it'll be updated.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <param name="value">A object to be stored to the ambient correlation list of objects.</param>
        void AddOrUpdateValue(TKey key, TValue value);
    }
}