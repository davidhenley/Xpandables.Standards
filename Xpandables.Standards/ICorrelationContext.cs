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
    /// This interface must be used or injected as a singleton dependency.
    /// </summary>
    public interface ICorrelationContext
    {
        /// <summary>
        /// Sets the correlated object by its key. If key already exist, it'll be updated.
        /// Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A object from the ambient correlation list of objects.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Optional<object> this[string key] { set; get; }

        /// <summary>
        /// Gets the correlated object by its key and returns a value when found.
        /// </summary>
        /// <typeparam name="T">Type of the object to return.</typeparam>
        /// <param name="key">The key of the correlated object.</param>
        /// <returns>A object from the ambient correlation list of objects.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        Optional<T> GetValue<T>(string key);

        /// <summary>
        /// Sets the correlated object by its key. If key already exist, it'll be updated.
        /// </summary>
        /// <typeparam name="T">Type of the object to be added or updated.</typeparam>
        /// <param name="key">The key of the correlated object.</param>
        /// <param name="value">A object to be stored to the ambient correlation list of objects.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        void SetOrUpdateValue<T>(string key, T value);

        /// <summary>
        /// Provides with the underlying dictionary.
        /// </summary>
        IReadOnlyDictionary<string, object> Collection { get; }
    }
}