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

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System
{
    /// <summary>
    /// The correlation collection implementation of <see cref="ICorrelationCollection{TKey, TValue}"/>.
    /// </summary>
    public class CorrelationCollection<TKey, TValue> : ICorrelationCollection<TKey, TValue>
    {
        protected AsyncLocal<ConcurrentDictionary<TKey, TValue>> Items { get; }

        /// <summary>
        /// Constructs a new instance that initializes the collection.
        /// </summary>
        public CorrelationCollection() => Items = new AsyncLocal<ConcurrentDictionary<TKey, TValue>>
        {
            Value = new ConcurrentDictionary<TKey, TValue>()
        };

        public virtual Optional<TValue> this[TKey key]
        {
            get
            {
                if (EqualityComparer<TKey>.Default.Equals(key, default))
                    return Optional<TValue>.Empty();

                return Items.Value.TryGetValue(key, out var found) ? found : default;
            }
            set
            {
                if (EqualityComparer<TKey>.Default.Equals(key, default)) return;
                if (EqualityComparer<TValue>.Default.Equals(value, default))
                    value = Optional<TValue>.Empty();

                Diagnostics.Contracts.Contract.Assume(!(value is null));

                Items.Value.AddOrUpdate(key, value.Cast<TValue>(), (_, __) => value.Cast<TValue>());
            }
        }

        public virtual void AddOrUpdateValue(TKey key, TValue value) => this[key] = value;

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Items.Value.GetEnumerator();

        public virtual Optional<TValue> GetValue(TKey key) => this[key];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}