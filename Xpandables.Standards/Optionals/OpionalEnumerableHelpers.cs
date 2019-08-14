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
using System.Linq;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Functionality for optional pattern for Enumerable.
    /// </summary>
    public static partial class Optional
    {
        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.FirstOrDefault();
        }

        public static Optional<T> FirstOrEmpty<T>(this IQueryable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.FirstOrDefault();
        }

        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => source.FirstOrDefault(predicate);

        public static Optional<T> FirstOrEmpty<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return source.FirstOrDefault(predicate);
        }

        public static IEnumerable<U> SelectOptional<T, U>(this IEnumerable<T> source, Func<T, Optional<U>> mapper)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));

            return from item in source
                   from result in mapper(item)
                   select result;
        }

        public static IQueryable<U> SelectOptional<T, U>(this IQueryable<T> source, Func<T, Optional<U>> mapper)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));

            return from item in source
                   from result in mapper(item)
                   select result;
        }

        public static IEnumerable<T> SelectOptional<T>(this IEnumerable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return from item in source
                   from result in item
                   select result;
        }

        public static IQueryable<T> SelectOptional<T>(this IQueryable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return from item in source
                   from result in item
                   select result;
        }

        public static IEnumerable<T> SelectOptional<T>(this IEnumerable<Optional<T>> source, Func<Optional<T>, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return from item in source
                   where predicate(item)
                   from result in item
                   select result;
        }

        public static IQueryable<T> SelectOptional<T>(this IQueryable<Optional<T>> source, Func<Optional<T>, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return from item in source
                   where predicate(item)
                   from result in item
                   select result;
        }

        //public static IAsyncEnumerable<T> SelectOptionalAsync<T>(this IEnumerable<IOptional<T>> source)
        //    => source.SelectOptionalAsync(item => item.hasValue);

        //public static IAsyncEnumerable<T> SelectOptionalAsync<T>(this IQueryable<IOptional<T>> source)
        //    => source.SelectOptionalAsync(item => item.hasValue);

        //public static IAsyncEnumerable<T> SelectOptionalAsync<T>(
        //    this IEnumerable<IOptional<T>> source, Func<IOptional<T>, bool> predicate)
        //    => source.SelectOptional(predicate).ToAsyncEnumerable();

        //public static IAsyncEnumerable<T> SelectOptionalAsync<T>(
        //    this IQueryable<IOptional<T>> source, Expression<Func<IOptional<T>, bool>> predicate)
        //    => source.SelectOptional(predicate).ToAsyncEnumerable();


        public static Optional<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.TryGetValue(key, out var value) ? Optional<TValue>.Some(value) : Optional<TValue>.Empty;

        public static Optional<T> TryGetElementAt<T>(this IEnumerable<T> source, int index)
            => source.ElementAtOrDefault(index);

        public static Optional<IEnumerable<TValue>> TryGetValues<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key)
            => lookup.Contains(key) ? Optional<IEnumerable<TValue>>.Some(lookup[key]) : Optional<IEnumerable<TValue>>.Empty;
    }
}