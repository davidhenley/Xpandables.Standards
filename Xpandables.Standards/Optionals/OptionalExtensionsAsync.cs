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
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionalities for optional pattern methods.
    /// </summary>
    public static partial class OptionalExtensions
    {
        /// <summary>
        /// Returns an optional that contains the value if that value matches the predicate.
        /// Otherwise returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value to act on.</param>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>An optional of <typeparamref name="T"/> value.</returns>
        public static async Task<Optional<T>> WhenAsync<T>(this T source, bool predicate)
        {
            return predicate
                ? await Task.FromResult(source.AsOptional()).ConfigureAwait(false)
                : Optional<T>.Empty();
        }

        /// <summary>
        /// Returns an optional that contains the value if that value matches the predicate.
        /// Otherwise returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value to act on.</param>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>An optional of <typeparamref name="T"/> value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static async Task<Optional<T>> WhenAsync<T>(this T source, Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return predicate(source)
                ? await Task.FromResult(source.AsOptional()).ConfigureAwait(false)
                : Optional<T>.Empty();
        }

        public static async Task MapAsync<T, U>(this Task<Optional<T>> optional, Func<T, Task<U>> some)
        {
            await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        public static async Task MapAsync<T>(this Task<Optional<T>> optional, Func<T, Task> some)
        {
            await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<U>> MapOptionalAsync<T, U>(
            this Task<Optional<T>> optional,
            Func<T, Task<Optional<U>>> some)
        {
            return await (await optional.ConfigureAwait(false)).MapOptionalAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenAsync<T>(
            this Task<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, Task<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async Task WhenAsync<T>(
            this Task<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, Task> some)
        {
            await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenEmptyAsync<T>(this Task<Optional<T>> optional, Func<Task<T>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }

        public static async Task<Optional<U>> WhenEmptyAsync<T, U>(this Task<Optional<T>> optional, Func<Task<U>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenEmptyOptionalAsync<T>(this Task<Optional<T>> optional, Func<Task<Optional<T>>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyOptionalAsync(empty).ConfigureAwait(false);
        }

        public static async Task<Optional<U>> WhenEmptyOptionalAsync<T, U>(this Task<Optional<T>> optional, Func<Task<Optional<U>>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyOptionalAsync(empty).ConfigureAwait(false);
        }

        public static async Task WhenEmptyAsync<T>(this Task<Optional<T>> optional, Func<Task> action)
        {
            await (await optional.ConfigureAwait(false)).WhenEmptyAsync(action).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenExceptionAsync<T>(
            this Task<Optional<T>> optional,
            Func<Task<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenExceptionOptionalAsync<T>(
            this Task<Optional<T>> optional,
            Func<Task<Optional<T>>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenExceptionAsync<T>(
            this Task<Optional<T>> optional,
            Func<Exception, Task<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        public static async Task<Optional<T>> WhenExceptionOptionalAsync<T>(
            this Task<Optional<T>> optional,
            Func<Exception, Task<Optional<T>>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        public static async Task WhenExceptionAsync<T>(
            this Task<Optional<T>> optional,
            Func<Exception, Task> some)
        {
            await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }
    }
}
