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

using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
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
        public static IEnumerable<T> ToEnumerable<T>(this T source)
        {
            if (EqualityComparer<T>.Default.Equals(source, default)) throw new ArgumentNullException(nameof(source));
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
    }
}