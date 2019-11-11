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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Extensions methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableHelpers
    {
        /// <summary>
        /// Converts a unique object to an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="source">An instance of the type.</param>
        /// <returns>An enumerable of the current instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="source"/> is an enumerable.</exception>
        public static IEnumerable<T> ToEnumerable<T>(this T source) where T : notnull
        {
            if (source.GetType().IsAssignableFrom(typeof(IEnumerable)))
                throw new ArgumentException(ErrorMessageResources.ToEnumerableArgument);

            yield return source;
        }

        /// <summary>
        /// Converts the <see cref="IEnumerable{T}"/> to a read only collection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="source">An instance of the collection to be converted.</param>
        /// <returns>A new <see cref="ReadOnlyCollection{T}"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
            => new ReadOnlyCollectionBuilder<T>(source).ToReadOnlyCollection();

        /// <summary>
        /// Returns the elements of the specified sequence or the value from the producer in a singleton
        /// collection if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="itemProducer">The delegate that produces the value.</param>
        /// <returns>A collection object that contains the default value for the TSource type if source is empty;
        /// otherwise, itemProducer value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="itemProducer"/> is null.</exception>
        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> source, Func<T> itemProducer)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (itemProducer is null) throw new ArgumentNullException(nameof(itemProducer));

            return source.DefaultIfEmpty(itemProducer());
        }

        /// <summary>
        /// Returns the first element of the specified sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The first element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.FirstOrDefault();
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element fo a condition.</param>
        /// <returns>The first element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return source.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Returns the last elements of a sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The last element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<T> LastOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.LastOrDefault();
        }

        /// <summary>
        /// Returns the last element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element fo a condition.</param>
        /// <returns>The last element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Optional<T> LastOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return source.LastOrDefault(predicate);
        }

        /// <summary>
        /// Returns the element at the specified index in a sequence or an empty optional if the index is out of range
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<T> TryGetElementAtOptional<T>(this IEnumerable<T> source, int index)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Projects each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static IEnumerable<TResult> SelectOptional<T, TResult>(this IEnumerable<T> source, Func<T, Optional<TResult>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return from item in source
                   from result in selector(item)
                   select result;
        }

        /// <summary>
        /// Projects asynchronously each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static async IAsyncEnumerable<TResult> SelectOptionalAsync<T, TResult>(this IAsyncEnumerable<T> source, Func<T, Optional<TResult>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            await foreach (var item in source)
            {
                foreach (var result in selector(item))
                    yield return result;
            }
        }

        /// <summary>
        /// Projects asynchronously each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the optional value function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async IAsyncEnumerable<T> SelectOptionalAsync<T>(this IAsyncEnumerable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            await foreach (var item in source)
            {
                foreach (var result in item)
                    yield return result;
            }
        }

        /// <summary>
        /// Projects asynchronously each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static async IAsyncEnumerable<T> SelectOptionalAsync<T>(this IAsyncEnumerable<Optional<T>> source, Func<Optional<T>, bool> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            await foreach (var item in source)
            {
                if (selector(item))
                {
                    foreach (var result in item)
                        yield return result;
                }
            }
        }

        /// <summary>
        /// Projects asynchronously each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static async IAsyncEnumerable<TResult> SelectOptionalAsync<T, TResult>(this IAsyncEnumerable<T> source, Func<T, ValueTask<Optional<TResult>>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            await foreach (var item in source)
            {
                foreach (var result in await selector(item).ConfigureAwait(false))
                    yield return result;
            }
        }

        /// <summary>
        /// Projects each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static IEnumerable<T> SelectOptional<T>(this IEnumerable<Optional<T>> source, Func<Optional<T>, bool> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return from item in source
                   where selector(item)
                   from result in item
                   select result;
        }

        /// <summary>
        /// Projects each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">A sequence of values to return only the exist values.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> whose elements are the result of no empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static IEnumerable<T> SelectOptional<T>(this IEnumerable<Optional<T>> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return from item in source
                   from result in item
                   select result;
        }

        /// <summary>
        /// Returns the element at the specified index in a sequence or an empty optional if the index is out of range
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async ValueTask<Optional<T>> TryGetElementAtOptionalAsync<T>(this IQueryable<T> source, int index)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return await new ValueTask<Optional<T>>(source.ElementAtOrDefault(index)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// if no found, return an empty optional.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">Represents a generic collection of key/value pairs.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dictionary"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public static Optional<TValue> TryGetValueOptional<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
                where TKey : notnull
        {
            if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.TryGetValue(key, out var value) ? value : default;
        }

        /// <summary>
        /// Gets the collection of values associated with the specified key.
        /// if no found, return an empty optional.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="lookup">Represents a generic collection of key/value pairs.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="lookup"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        public static Optional<IEnumerable<TValue>> TryGetValuesOptional<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key)
                where TKey : notnull
        {
            if (lookup is null) throw new ArgumentNullException(nameof(lookup));
            return lookup.Contains(key) ? Optional<IEnumerable<TValue>>.Some(lookup[key]) : Optional<IEnumerable<TValue>>.Empty();
        }

        /// <summary>
        /// Enumerates the sequence and invokes the given action for each value in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <returns>Collection of items.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified
        /// outside the process.</exception>
        public static IEnumerable<TResult> ForEach<T, TResult>(this IEnumerable<T> source, Func<T, TResult> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            return from item in source
                   select action(item);
        }

        /// <summary>
        /// Enumerates the sequence and invokes the given action for each value in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <returns>Collection of items.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified
        /// outside the process.</exception>
        public static async IAsyncEnumerable<TResult> ForEachAsync<T, TResult>(this IAsyncEnumerable<T> source, Func<T, TResult> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            await foreach (var item in source)
                yield return action(item);
        }

        /// <summary>
        /// Enumerates the sequence and invokes the given action for each value in the sequence using an index.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <returns>Collection of items.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified
        /// outside the process.</exception>
        public static IEnumerable<TResult> ForEach<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            return from item in source.Select((Value, Index) => new { Index, Value })
                   select action(item.Value, item.Index);
        }

        /// <summary>
        /// Enumerates the sequence and invokes the given action for each value in the sequence using an index.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <returns>Collection of items.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified
        /// outside the process.</exception>
        public static async IAsyncEnumerable<TResult> ForEachAsync<T, TResult>(this IAsyncEnumerable<T> source, Func<T, int, TResult> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            int index = 0;
            await foreach (var item in source)
                yield return action(item, index++);
        }

        /// <summary>
        /// Projects each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static async IAsyncEnumerable<TResult> SelectOptionalAsync<T, TResult>(this IQueryable<T> source, Func<T, ValueTask<Optional<TResult>>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            foreach (var item in source)
            {
                foreach (var result in await selector(item).ConfigureAwait(false))
                    yield return result;
            }
        }

        /// <summary>
        /// Projects each element of the sequence in a new form.
        /// </summary>
        /// <typeparam name="T">The type of the element in the sequence.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        public static async IAsyncEnumerable<T> SelectOptionalAsync<T>(this IQueryable<Optional<T>> source, Func<Optional<T>, ValueTask<bool>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            foreach (var item in source)
            {
                if (await selector(item).ConfigureAwait(false))
                {
                    foreach (var result in item)
                        yield return result;
                }
            }
        }

        /// <summary>
        /// Enumerates the sequence and invokes the given action for each value in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of the element in the sequence.</typeparam>
        /// <typeparam name="TResult">Type of the result element.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An element of the collection has be modified outside the process.</exception>
        public static Optional<IEnumerable<TResult>> ForEachOptional<T, TResult>(this Optional<IEnumerable<T>> source, Func<T, Optional<TResult>> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                var results = new HashSet<TResult>();
                foreach (var element in item)
                {
                    foreach (var result in action(element))
                        results.Add(result);
                }

                return Optional<IEnumerable<TResult>>.Some(results.AsEnumerable());
            }

            return Optional<IEnumerable<TResult>>.Empty();
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
        public static void ForEachOptional<T>(this Optional<List<T>> source, Action<T> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
                item.ForEach(action);
        }
    }
}