using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace System
{
    /// <summary>
    /// Functionality for optional pattern for Enumerable.
    /// </summary>
    public static class OptionalEnumerableHelpers
    {
        public static async Task<Optional<T>> FirstOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task<Optional<T>> LastOrEmptyAsync<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await source.LastOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task<Optional<T>> FirstOrEmptyAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate)
            => await source.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
    }
}
