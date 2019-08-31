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

using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public partial class Optional<T>
    {
        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            return this.Any()
                ? await some(this.Single()).ConfigureAwait(false)
                : default;
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task MapAsync(Func<T, Task> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (this.Any()) await some(this.Single()).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a pair optional pair with the second instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the second instance</typeparam>
        /// <param name="second">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is null.</exception>
        public async Task<Optional<(Task<Optional<T>> First, Task<Optional<TResult>> Second)>> AndAsync<TResult>(
            Func<Task<Optional<TResult>>> second)
        {
            if (second is null) throw new ArgumentNullException(nameof(second));
            var secondPart = await second().ConfigureAwait(false);
            return (this, secondPart);
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <typeparam name="TResult">The expected type of result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<TResult>> MapOptionalAsync<TResult>(Func<T, Task<Optional<TResult>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            return this.Any()
                ? await some(this.Single()).ConfigureAwait(false)
                : Optional<TResult>.Empty;
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element when predicate is true.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<TResult>> WhenAsync<TResult>(Predicate<T> predicate, Func<T, Task<TResult>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single()))
                return await some(this.Single()).ConfigureAwait(false);

            return Optional<TResult>.Empty;
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element when predicate is true.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task WhenAsync(Predicate<T> predicate, Func<T, Task> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single()))
                await some(this.Single()).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceAsync(Func<Task<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return this.Any()
                ? this.Single()
                : await empty().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceOptionalAsync(Func<Task<Optional<T>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return this.Any()
                ? this
                : await empty().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task ReduceAsync(Func<Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (!this.Any()) await action().ConfigureAwait(false);
        }
    }
}