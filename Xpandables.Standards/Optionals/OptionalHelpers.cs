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

using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionalities for optional pattern methods.
    /// </summary>
    public static partial class Optional
    {
        public static async Task MapAsync<T, U>(
            this Task<Optional<T>> optional,
            Func<T, CancellationToken, Task<U>> some,
            Func<T, CancellationToken, Task<U>> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .MapAsync(some, empty, cancellationToken).ConfigureAwait(false);

        public static async Task MapAsync<T>(
            this Task<Optional<T>> optional,
            Func<T, CancellationToken, Task> some,
            Func<T, CancellationToken, Task> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .MapAsync(some, empty, cancellationToken).ConfigureAwait(false);

        public static async Task<Optional<(Task<Optional<T>> First, Task<Optional<U>> Second)>> AndAsync<T, U>(
            this Task<Optional<T>> optional,
            Func<Optional<T>, CancellationToken, Task<Optional<U>>> second,
            CancellationToken cancellationToken)
              => await (await optional.ConfigureAwait(false))
                .AndAsync(second, cancellationToken).ConfigureAwait(false);

        public static async Task<Optional<U>> MapOptionalAsync<T, U>(
            this Task<Optional<T>> optional,
            Func<T, CancellationToken, Task<Optional<U>>> some,
             Func<T, CancellationToken, Task<Optional<U>>> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .MapOptionalAsync(some, empty, cancellationToken).ConfigureAwait(false);

        public static async Task<Optional<U>> WhenAsync<T, U>(
            this Task<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, CancellationToken, Task<U>> some,
            Func<T, CancellationToken, Task<U>> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some, empty, cancellationToken).ConfigureAwait(false);

        public static async Task WhenAsync<T>(
            this Task<Optional<T>> optional,
            Predicate<T> predicate,
            Func<T, CancellationToken, Task> some,
            Func<T, CancellationToken, Task> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .WhenAsync(predicate, some, empty, cancellationToken).ConfigureAwait(false);

        public static async Task<Optional<T>> ReduceAsync<T>(
            this Task<Optional<T>> optional,
            Func<CancellationToken, Task<T>> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .ReduceAsync(empty, cancellationToken).ConfigureAwait(false);

        public static async Task<Optional<T>> ReduceOptionalAsync<T>(
            this Task<Optional<T>> optional,
            Func<CancellationToken, Task<Optional<T>>> empty,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .ReduceOptionalAsync(empty, cancellationToken).ConfigureAwait(false);

        public static async Task ReduceAsync<T>(
            this Task<Optional<T>> optional,
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken)
            => await (await optional.ConfigureAwait(false))
                .ReduceAsync(action, cancellationToken).ConfigureAwait(false);
   }
}