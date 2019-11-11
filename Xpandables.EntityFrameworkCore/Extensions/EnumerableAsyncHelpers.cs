/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Extensions async methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableAsyncHelpers
    {
        /// <summary>
        /// Asynchronously returns the first element of the specified sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The first element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async ValueTask<Optional<T>> FirstOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the first element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element fo a condition.</param>
        /// <returns>The first element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static async ValueTask<Optional<T>> FirstOrEmptyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return await source.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the last elements of a sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The last element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async ValueTask<Optional<T>> LastOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.LastOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously returns the last element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element fo a condition.</param>
        /// <returns>The last element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static async ValueTask<Optional<T>> LastOrEmptyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return await source.LastOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">The System.Action`1 delegate to perform on each element of the <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified outside the process.</exception>
        public static async Task ForEachOptionalAsync<T>(this Optional<IQueryable<T>> source, Action<T> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
                await item.ForEachAsync(action).ConfigureAwait(false);
        }
    }
}