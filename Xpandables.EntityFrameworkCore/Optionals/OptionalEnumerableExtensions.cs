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

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionality for optional pattern for Enumerable.
    /// </summary>
    public static class OptionalEnumerableExtensions
    {
        public static async Task<Optional<T>> FirstOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task<Optional<T>> FirstOrEmptyAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate)
            => await source.FirstOrDefaultAsync(predicate).ConfigureAwait(false);

        public static async Task<Optional<T>> LastOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.LastOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
