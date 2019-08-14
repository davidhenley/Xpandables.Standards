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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Xpandables;

namespace System
{
    /// <summary>
    /// The correlation context provider.
    /// </summary>
    public sealed class CorrelationContext : Explicit<ICorrelationContext>, ICorrelationContext
    {
        private readonly AsyncLocal<ConcurrentDictionary<string, object>> _items;

        /// <summary>
        /// Constructs a new instance that initializes the collection.
        /// </summary>
        public CorrelationContext() => _items = new AsyncLocal<ConcurrentDictionary<string, object>>
        {
            Value = new ConcurrentDictionary<string, object>()
        };

        Optional<object> ICorrelationContext.this[string key]
        {
            get => _items.Value.TryGetValue(key, out var value) ? value : default;
            set
            {
                if (_items.Value.TryGetValue(key, out var foundValue))
                    _items.Value.TryUpdate(key, value, foundValue);
                else
                    _items.Value.TryAdd(key, value);
            }
        }

        IReadOnlyDictionary<string, object> ICorrelationContext.Collection => _items.Value;

        Optional<T> ICorrelationContext.GetValue<T>(string key) => Instance[key].Cast<T>();

        void ICorrelationContext.SetOrUpdateValue<T>(string key, T value) => Instance[key] = value;
    }
}