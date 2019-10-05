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
        public static async ValueTask<Optional<T>> WhenAsync<T>(this T source, bool predicate)
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
        public static async ValueTask<Optional<T>> WhenAsync<T>(this T source, Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return predicate(source)
                ? await Task.FromResult(source.AsOptional()).ConfigureAwait(false)
                : Optional<T>.Empty();
        }

        /// <summary>
        /// Converts the specified value to an optional instance.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>An optional instance.</returns>
        public static async ValueTask<Optional<T>> AsOptionalAsync<T>(this T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default)) return Optional<T>.Empty();
            return await Task.FromResult(Optional<T>.Some(value)).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts the specified value to an optional pair instance.
        /// if one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <typeparam name="U">The type of the right value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <param name="right">The right value to act on.</param>
        /// <returns>An optional pair instance.</returns>
        public static async ValueTask<Optional<(T Left, U Right)>> AsOptionalAsync<T, U>(this T value, U right)
            => await Task.FromResult(value.AsOptional(right)).ConfigureAwait(false);

        /// <summary>
        /// Converts the specified optional to an optional pair instance.
        /// if one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <typeparam name="U">The type of the right value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="right">The right value to act on.</param>
        /// <returns>An optional pair instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        public static ValueTask<Optional<(T Left, U Right)>> AsOptionalAsync<T, U>(this Optional<T> optional, U right)
            => new ValueTask<Optional<(T Left, U Right)>>(optional.AsOptional(right));

        public static async Task MapAsync<T, U>(this ValueTask<Optional<T>> optional, Func<T, ValueTask<U>> some)
        {
            await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        public static async Task MapAsync<T>(this ValueTask<Optional<T>> optional, Func<T, Task> some)
        {
            await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<(T Left, U Right)>> AndOptionalAsync<T, U>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<Optional<U>>> second)
        {
            return await (await optional.ConfigureAwait(false)).AndOptionalAsync(second).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<(T Left, U Right)>> AndAsync<T, U>(
           this ValueTask<Optional<T>> optional,
           Func<T, ValueTask<U>> second)
        {
            return await (await optional.ConfigureAwait(false)).AndAsync(second).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<(T Left, U Right)>> AndAsync<T, U>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<U>> second)
        {
            return await (await optional.ConfigureAwait(false)).AndAsync(second).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<U>> MapOptionalAsync<T, U>(
            this ValueTask<Optional<T>> optional,
            Func<T, ValueTask<Optional<U>>> some)
        {
            return await (await optional.ConfigureAwait(false)).MapOptionalAsync(some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenAsync<T>(
            this ValueTask<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, ValueTask<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async Task WhenAsync<T>(
            this ValueTask<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, Task> some)
        {
            await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenEmptyAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<T>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenEmptyOptionalAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<Optional<T>>> empty)
        {
            return await (await optional.ConfigureAwait(false)).WhenEmptyOptionalAsync(empty).ConfigureAwait(false);
        }

        public static async Task WhenEmptyAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<Task> action)
        {
            await (await optional.ConfigureAwait(false)).WhenEmptyAsync(action).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenExceptionAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenExceptionOptionalAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<ValueTask<Optional<T>>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenExceptionAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<Exception, ValueTask<T>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        public static async ValueTask<Optional<T>> WhenExceptionOptionalAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<Exception, ValueTask<Optional<T>>> some)
        {
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        public static async Task WhenExceptionAsync<T>(
            this ValueTask<Optional<T>> optional,
            Func<Exception, Task> some)
        {
            await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }
    }
}
