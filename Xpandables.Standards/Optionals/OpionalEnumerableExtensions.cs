﻿/************************************************************************************************************
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
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionality for optional pattern for Enumerable.
    /// </summary>
    public static partial class OptionalExtensions
    {
        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.FirstOrDefault();
        }

        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return source.FirstOrDefault(predicate);
        }

        public static Task<Optional<T>> FirstOrEmptyAsync<T>(this IAsyncEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.FirstOrEmptyAsync(_ => true);
        }

        public static async Task<Optional<T>> FirstOrEmptyAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            var result = Optional<T>.Empty();
            await foreach (var item in source)
            {
                if (predicate(item))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        public static Optional<T> LastOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.LastOrDefault();
        }

        public static IEnumerable<U> SelectOptional<T, U>(this IEnumerable<T> source, Func<T, Optional<U>> mapper)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));

            return from item in source
                   from result in mapper(item)
                   select result;
        }

        public static async IAsyncEnumerable<U> SelectOptionalAsync<T, U>(
            this IAsyncEnumerable<T> source,
            Func<T, Optional<U>> mapper)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));

            await foreach (var item in source)
            {
                var result = mapper(item);
                if (result.IsValue())
                    yield return result;
            }
        }

        public static IEnumerable<T> SelectOptional<T>(this IEnumerable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return from item in source
                   from result in item
                   select result;
        }

        public static async IAsyncEnumerable<T> SelectOptionalAsync<T>(this IAsyncEnumerable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            await foreach (var item in source)
            {
                if (item.IsValue())
                    yield return item;
            }
        }

        public static IEnumerable<T> SelectOptional<T>(
            this IEnumerable<Optional<T>> source,
            Func<Optional<T>, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return from item in source
                   where predicate(item)
                   from result in item
                   select result;
        }

        public static async IAsyncEnumerable<T> SelectOptionalAsync<T>(
            this IAsyncEnumerable<Optional<T>> source,
            Func<Optional<T>, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            await foreach (var item in source)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        public static Optional<TValue> TryGetValueOptional<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default)) throw new ArgumentNullException(nameof(key));
            if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value)
                ? value
                : default;
        }

        public static Optional<T> TryGetElementAtOptional<T>(this IEnumerable<T> source, int index)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.ElementAtOrDefault(index);
        }

        public static Optional<IEnumerable<TValue>> TryGetValuesExtended<TKey, TValue>(
            this ILookup<TKey, TValue> lookup,
            TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default)) throw new ArgumentNullException(nameof(key));
            if (lookup is null) throw new ArgumentNullException(nameof(lookup));

            return lookup.Contains(key)
                ? Optional<IEnumerable<TValue>>.Some(lookup[key])
                : Optional<IEnumerable<TValue>>.Empty();
        }

        public static void ForEachOptional<TSource, TElement>(this Optional<TSource> optional, Action<TElement> some)
            where TSource : IEnumerable<TElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.IsValue())
            {
                optional.InternalValue
                    .ToList()
                    .ForEach(some);
            }
        }

        public static Optional<TSource> ForEachOptional<TSource, TElement>(
            this Optional<TSource> optional,
            Func<TElement, TElement> some)
            where TSource : IEnumerable<TElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.IsValue())
            {
                var result = new HashSet<TElement>();
                foreach (var element in optional.InternalValue)
                    result.Add(some(element));

                return (TSource)result.AsEnumerable();
            }

            return optional;
        }

        public static Optional<TResult> ForEachOptional<TSource, TResult, TSourceElement, TResultElement>(
            this Optional<TSource> optional,
            Func<TSourceElement, TResultElement> some)
            where TSource : IEnumerable<TSourceElement>
            where TResult : IEnumerable<TResultElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.IsValue())
            {
                var result = new HashSet<TResultElement>();
                foreach (var element in optional.InternalValue)
                    result.Add(some(element));

                return (TResult)result.AsEnumerable();
            }

            return Optional<TResult>.Empty();
        }
    }
}