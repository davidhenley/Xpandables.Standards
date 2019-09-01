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
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionalities for optional pattern methods.
    /// </summary>
    public static partial class OptionalHelpers
    {
        /// <summary>
        /// Converts the specified value to an optional instance.
        /// </summary>
        /// <typeparam name="TValue">The Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<TValue> ToOptional<TValue>(this TValue value)
        {
            if (value is null)
                return Optional<TValue>.Empty;

            return Optional<TValue>.Some(value);
        }

        /// <summary>
        /// Converts the specified object to an optional instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        /// <param name="source">The object to be converted.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<TResult> ToOptional<TResult>(this object source)
        {
            if (source is TResult result)
                return Optional<TResult>.Some(result);

            return Optional<TResult>.Empty;
        }

        public static async Task MapAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<TValue, Task<TResult>> some)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .MapAsync(some).ConfigureAwait(false);
        }

        public static async Task MapAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<TValue, Task> some)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .MapAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<OptionalPair<TValue, TResult>>>
            AndAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<Task<Optional<TResult>>> second)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second).ConfigureAwait(false);
        }

        public static async Task<Optional<OptionalPair<TValue, TResult>>>
          AndAsync<TValue, TResult>(
          this Task<Optional<TValue>> optional,
          Func<TValue, Task<Optional<TResult>>> second)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second).ConfigureAwait(false);
        }

        public static async Task<Optional<OptionalPair<TValue, TResult>>>
          AndAsync<TValue, TResult>(
          this Task<Optional<TValue>> optional,
          Func<Optional<TValue>, Task<Optional<TResult>>> second)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second).ConfigureAwait(false);
        }

        public static async Task<Optional<OptionalPair<TValue, TResult>>>
           AndAsync<TValue, TResult>(
           this Task<Optional<TValue>> optional,
           Func<TValue, Task<TResult>> second)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second).ConfigureAwait(false);
        }

        public static async Task<Optional<OptionalPair<TValue, TResult>>>
        AndAsync<TValue, TResult>(
        this Task<Optional<TValue>> optional,
        Func<Task<TResult>> second)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second).ConfigureAwait(false);
        }

        public static async Task<Optional<TResult>> MapOptionalAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<TValue, Task<Optional<TResult>>> some)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .MapOptionalAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<TResult>> WhenAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Predicate<TValue> predicate,
            Func<TValue, Task<TResult>> some)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async Task WhenAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Predicate<TValue> predicate,
            Func<TValue, Task> some)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async Task<Optional<TValue>> ReduceAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<Task<TValue>> empty)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .ReduceAsync(empty).ConfigureAwait(false);
        }

        public static async Task<Optional<TValue>> ReduceOptionalAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<Task<Optional<TValue>>> empty)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .ReduceOptionalAsync(empty).ConfigureAwait(false);
        }

        public static async Task ReduceAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<Task> action)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .ReduceAsync(action).ConfigureAwait(false);
        }

        public static void ForEach<TSource, TElement>(this Optional<TSource> optional, Action<TElement> some)
           where TSource : IEnumerable<TElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.Any())
                optional.Single()
                    .ToList()
                    .ForEach(some);
        }

        public static Optional<TSource> ForEach<TSource, TElement>(
            this Optional<TSource> optional,
            Func<TElement, TElement> some)
            where TSource : IEnumerable<TElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.Any())
            {
                var result = new List<TElement>();
                foreach (var element in optional.Single())
                    result.Add(some(element));

                return result.ToOptional<TSource>();
            }

            return optional;
        }

        public static Optional<TResult> ForEach<TSource, TResult, TSourceElement, TResultElement>(
            this Optional<TSource> optional,
            Func<TSourceElement, TResultElement> some)
            where TSource : IEnumerable<TSourceElement>
            where TResult : IEnumerable<TResultElement>
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (optional.Any())
            {
                var result = new List<TResultElement>();
                foreach (var element in optional.Single())
                    result.Add(some(element));

                return result.ToOptional<TResult>();
            }

            return Enumerable.Empty<TResultElement>().ToOptional<TResult>();
        }
    }
}