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
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionalities for optional pattern methods.
    /// </summary>
    public static partial class OptionalHelpers
    {
        /// <summary>
        /// Provides with an optional without value.
        /// </summary>
        /// <typeparam name="TValue">The Type of the value.</typeparam>
        /// <returns>An empty optional.</returns>
        public static Optional<TValue> Empty<TValue>() => new Optional<TValue>(Array.Empty<TValue>());

        /// <summary>
        /// Provides with an optional that contains a value.
        /// </summary>
        /// <typeparam name="TValue">The Type of the value.</typeparam>
        /// <param name="value">The value to be used for optional.</param>
        /// <returns>An optional with a value.</returns>
        public static Optional<TValue> Some<TValue>(TValue value) => new Optional<TValue>(new TValue[] { value });

        /// <summary>
        /// Converts the specified value to an optional instance.
        /// </summary>
        /// <typeparam name="TValue">The Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<TValue> ToOptional<TValue>(this TValue value)
        {
            if (value is null)
                return Empty<TValue>();

            return Some(value);
        }

        public static async Task MapAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<TValue, CancellationToken, Task<TResult>> some,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .MapAsync(some, cancellationToken).ConfigureAwait(false);
        }

        public static async Task MapAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<TValue, CancellationToken, Task> some,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .MapAsync(some, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Optional<(Task<Optional<TValue>> First, Task<Optional<TResult>> Second)>>
            AndAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<Optional<TValue>, CancellationToken, Task<Optional<TResult>>> second,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .AndAsync(second, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Optional<TResult>> MapOptionalAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Func<TValue, CancellationToken, Task<Optional<TResult>>> some,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .MapOptionalAsync(some, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Optional<TResult>> WhenAsync<TValue, TResult>(
            this Task<Optional<TValue>> optional,
            Predicate<TValue> predicate,
            Func<TValue, CancellationToken, Task<TResult>> some,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some, cancellationToken).ConfigureAwait(false);
        }

        public static async Task WhenAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Predicate<TValue> predicate,
            Func<TValue, CancellationToken, Task> some,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Optional<TValue>> ReduceAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<CancellationToken, Task<TValue>> empty,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .ReduceAsync(empty, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Optional<TValue>> ReduceOptionalAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<CancellationToken, Task<Optional<TValue>>> empty,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false))
                .ReduceOptionalAsync(empty, cancellationToken).ConfigureAwait(false);
        }

        public static async Task ReduceAsync<TValue>(
            this Task<Optional<TValue>> optional,
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken)
        {
            if (optional is null)
                throw new ArgumentNullException(nameof(optional));

            await (await optional.ConfigureAwait(false))
                .ReduceAsync(action, cancellationToken).ConfigureAwait(false);
        }
    }
}