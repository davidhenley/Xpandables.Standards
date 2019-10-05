
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

using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace System
{
    /// <summary>
    /// Provides with cache behavior to <see cref="IInstanceCreator"/>.
    /// You must derive from this class to implement custom behavior cache.
    /// </summary>
    public class InstanceCreatorCache : InstanceCreator
    {
        private readonly IMemoryCache _cache;

        public InstanceCreatorCache(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override TDelegate GetLambdaConstructor<TDelegate>(Type type, params Type[] parameterTypes)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            var key = KeyBuilder(type, parameterTypes);

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                return base.GetLambdaConstructor<TDelegate>(type, parameterTypes);
            });
        }

        protected virtual string KeyBuilder(Type type, params Type[] parameterTypes)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            var key = type.Name;
            if (parameterTypes.Length > 0) key += string.Concat(parameterTypes.Select(t => t.Name));

            return key;
        }
    }
}
