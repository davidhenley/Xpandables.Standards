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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Xpandables.EntityFrameworkCore")]

namespace System.Design.Database
{
    public partial interface IDataContext
    {
        internal Task<bool> AnyAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);

        internal Task<bool> AnyAsync<T>(
            [NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        internal Task<bool> AllAsync<T>(
            [NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        internal Task<int> CountAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);

        internal Task<int> CountAsync<T>(
            [NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        internal Task<long> LongCountAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<long> LongCountAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> FirstAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> FirstAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> FirstOrDefaultAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> FirstOrDefaultAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> LastAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> LastAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> LastOrDefaultAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> LastOrDefaultAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> SingleAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> SingleAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> SingleOrDefaultAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T> SingleOrDefaultAsync<T>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        internal Task<T> MinAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<TResult> MinAsync<T, TResult>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
        internal Task<T> MaxAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<TResult> MaxAsync<T, TResult>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
        internal Task<U> SumAsync<U>([NotNull] IQueryable<U> source, CancellationToken cancellationToken = default) where U : unmanaged;
        internal Task<U?> SumAsync<U>([NotNull] IQueryable<U?> source, CancellationToken cancellationToken = default) where U : unmanaged;
        internal Task<U> SumAsync<T, U>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, U>> selector, CancellationToken cancellationToken = default) where T : class where U : unmanaged;
        internal Task<U?> SumAsync<T, U>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, U?>> selector, CancellationToken cancellationToken = default) where T : class where U : unmanaged;
        internal Task<U> AverageAsync<U>([NotNull] IQueryable<U> source, CancellationToken cancellationToken = default) where U : unmanaged;
        internal Task<U?> AverageAsync<U>([NotNull] IQueryable<U?> source, CancellationToken cancellationToken = default) where U : unmanaged;
        internal Task<U> AverageAsync<T, U>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, U>> selector, CancellationToken cancellationToken = default) where T : class where U : unmanaged;
        internal Task<U?> AverageAsync<T, U>([NotNull] IQueryable<T> source, [NotNull] Expression<Func<T, U?>> selector, CancellationToken cancellationToken = default) where T : class where U : unmanaged;
        internal Task<bool> ContainsAsync<T>([NotNull] IQueryable<T> source, [NotNull] T item, CancellationToken cancellationToken = default);
        internal Task<List<T>> ToListAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal Task<T[]> ToArrayAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default);
        internal IQueryable<T> Include<T>([NotNull] IQueryable<T> source, [NotNull] string navigationPropertyPath) where T : class;
        internal IQueryable<T> IgnoreQueryFilters<T>([NotNull] IQueryable<T> source) where T : class;
        internal IQueryable<T> AsNoTracking<T>([NotNull] IQueryable<T> source) where T : class;
        internal IQueryable<T> AsTracking<T>([NotNull] IQueryable<T> source) where T : class;
        internal IQueryable<T> TagWith<T>([NotNull] IQueryable<T> source, [NotNull] string tag);
        internal void Load<T>([NotNull] IQueryable<T> source) where T : class;
        internal Task LoadAsync<T>([NotNull] IQueryable<T> source, CancellationToken cancellationToken = default) where T : class;

        internal Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>([NotNull] IQueryable<T> source, [NotNull] Func<T, TKey> keySelector, CancellationToken cancellationToken = default)
            where T : class;

        internal Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>([NotNull] IQueryable<T> source,
            [NotNull] Func<T, TKey> keySelector, [NotNull] IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default) where T : class;

        internal Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>([NotNull] IQueryable<T> source,
            [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, TElement> elementSelector, CancellationToken cancellationToken = default) where T : class;

        internal Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>([NotNull] IQueryable<T> source,
            [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, TElement> elementSelector, [NotNull] IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default) where T : class;

        internal Task ForEachAsync<T>([NotNull] IQueryable<T> source, [NotNull] Action<T> action, CancellationToken cancellationToken = default) where T : class;
        internal IAsyncEnumerable<T> AsAsyncEnumerable<T>([NotNull] IQueryable<T> source) where T : class;
    }
}
