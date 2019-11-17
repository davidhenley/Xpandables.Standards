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
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Database.Extensions
{
    /// <summary>
    /// Provides with EntityFrameworkCore <see cref="IDataContext"/> extension methods.
    /// </summary>
    public static class DataContextExtentions
    {
        public static async Task<bool> AnyAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.AnyAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<bool> AnyAsync<T>
            ([NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.AnyAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<bool> AllAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.AllAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<int> CountAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.CountAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<int> CountAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.CountAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<long> LongCountAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LongCountAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<long> LongCountAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LongCountAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> FirstAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.FirstAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> FirstAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.FirstAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> FirstOrDefaultAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> FirstOrDefaultAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.FirstOrDefaultAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> LastAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LastAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> LastAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LastAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> LastOrDefaultAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LastOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> LastOrDefaultAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LastOrDefaultAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> SingleAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.SingleAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> SingleAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.SingleAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> SingleOrDefaultAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.SingleOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<T> SingleOrDefaultAsync<T>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.SingleOrDefaultAsync(source, predicate, cancellationToken).ConfigureAwait(false);

        public async static Task<T> MinAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.MinAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<TResult> MinAsync<T, TResult>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.MinAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<T> MaxAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.MaxAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<TResult> MaxAsync<T, TResult>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.MaxAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<U> SumAsync<U>([NotNull] this IQueryable<U> source, CancellationToken cancellationToken = default)
            where U : unmanaged
            => await ((IDataQueryable<U>)source).Instance.SumAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<U?> SumAsync<U>([NotNull] this IQueryable<U?> source, CancellationToken cancellationToken = default)
            where U : unmanaged
            => await ((IDataQueryable<U>)source).Instance.SumAsync(source, cancellationToken).ConfigureAwait(false);

        public static async Task<U> SumAsync<T, U>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, U>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            where U : unmanaged
            => await ((IDataQueryable<T>)source).Instance.SumAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<U?> SumAsync<T, U>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, U?>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            where U : unmanaged
            => await ((IDataQueryable<T>)source).Instance.SumAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<U> AverageAsync<U>([NotNull] this IQueryable<U> source, CancellationToken cancellationToken = default)
            where U : unmanaged
            => await ((IDataQueryable<U>)source).Instance.AverageAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<U?> AverageAsync<U>([NotNull] this IQueryable<U?> source, CancellationToken cancellationToken = default)
            where U : unmanaged
            => await ((IDataQueryable<U>)source).Instance.AverageAsync(source, cancellationToken).ConfigureAwait(false);

        public async static Task<U> AverageAsync<T, U>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, U>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            where U : unmanaged
            => await ((IDataQueryable<T>)source).Instance.SumAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<U?> AverageAsync<T, U>(
            [NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, U?>> selector, CancellationToken cancellationToken = default)
            where T : Entity
            where U : unmanaged
            => await ((IDataQueryable<T>)source).Instance.AverageAsync(source, selector, cancellationToken).ConfigureAwait(false);

        public async static Task<bool> ContainsAsync<T>([NotNull] this IQueryable<T> source, [NotNull] T item, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ContainsAsync(source, item, cancellationToken).ConfigureAwait(false);

        public static async Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToListAsync(source, cancellationToken).ConfigureAwait(false);

        public static async Task<T[]> ToArrayAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToArrayAsync(source, cancellationToken).ConfigureAwait(false);

        public static IQueryable<T> Include<T>([NotNull] this IQueryable<T> source, [NotNull] string navigationPropertyPath)
            where T : Entity
            => ((IDataQueryable<T>)source).Instance.Include(source, navigationPropertyPath);

        public static IQueryable<T> Include<T, TProperty>([NotNull] this IQueryable<T> source, [NotNull] Expression<Func<T, TProperty>> navigationPropertyPath)
            where T : Entity
        {
            if (navigationPropertyPath is null) throw new ArgumentNullException(nameof(navigationPropertyPath));
            if (navigationPropertyPath.Body is ConstantExpression constantExpression)
                return ((IDataQueryable<T>)source).Instance.Include(source, constantExpression.Value.ToString());

            var expressionMember = (navigationPropertyPath.Body as MemberExpression
              ?? ((UnaryExpression)navigationPropertyPath.Body).Operand as MemberExpression)
              ?.Member.Name;

            if (expressionMember is string)
                return ((IDataQueryable<T>)source).Instance.Include(source, expressionMember);

            throw new ArgumentException(nameof(navigationPropertyPath));
        }

        public static IQueryable<T> IgnoreQueryFilters<T>([NotNull] this IQueryable<T> source) where T : Entity
            => ((IDataQueryable<T>)source).Instance.IgnoreQueryFilters(source);

        public static IQueryable<T> AsNoTracking<T>([NotNull] this IQueryable<T> source) where T : Entity
            => ((IDataQueryable<T>)source).Instance.AsNoTracking(source);

        public static IQueryable<T> AsTracking<T>([NotNull] this IQueryable<T> source) where T : Entity
            => ((IDataQueryable<T>)source).Instance.AsTracking(source);

        public static IQueryable<T> TagWith<T>([NotNull] this IQueryable<T> source, [NotNull]  string tag)
            where T : Entity
            => ((IDataQueryable<T>)source).Instance.TagWith(source, tag);

        public static void Load<T>([NotNull] this IQueryable<T> source) where T : Entity
            => ((IDataQueryable<T>)source).Instance.Load(source);

        public static async Task LoadAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.LoadAsync(source, cancellationToken).ConfigureAwait(false);

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
            [NotNull] this IQueryable<T> source, [NotNull] Func<T, TKey> keySelector, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToDictionaryAsync(source, keySelector, cancellationToken).ConfigureAwait(false);

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
            [NotNull] this IQueryable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToDictionaryAsync(source, keySelector, comparer, cancellationToken).ConfigureAwait(false);

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(
            [NotNull] this IQueryable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, TElement> elementSelector,
            CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToDictionaryAsync(source, keySelector, elementSelector, cancellationToken).ConfigureAwait(false);

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>([NotNull] this IQueryable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, TElement> elementSelector, [NotNull] IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ToDictionaryAsync(source, keySelector, elementSelector, comparer, cancellationToken).ConfigureAwait(false);

        public static async Task ForEachAsync<T>([NotNull] this IQueryable<T> source, [NotNull] Action<T> action, CancellationToken cancellationToken = default)
            where T : Entity
            => await ((IDataQueryable<T>)source).Instance.ForEachAsync(source, action, cancellationToken).ConfigureAwait(false);

        public static IAsyncEnumerable<T> AsAsyncEnumerable<T>([NotNull] this IQueryable<T> source)
            where T : Entity
            => ((IDataQueryable<T>)source).Instance.AsAsyncEnumerable(source);
    }
}
