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
        /// Returns the underlying value.
        /// If optional is empty, returns the default type of <typeparamref name="T"/>.
        /// <para>if <typeparamref name="T"/> is not nullable, be aware of exception.</para>
        /// </summary>
        public async Task<T> ReturnAsync() => await Task.FromResult(Cast<T>()).ConfigureAwait(false);

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
        /// Creates an optional pair with the second instance.
        /// if one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public async Task<Optional<OptionalPair<T, U>>> AndAsync<U>(Func<T, Task<Optional<U>>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = await right(this).ConfigureAwait(false);
            return second.Any()
                ? Optional<OptionalPair<T, U>>.Some(new OptionalPair<T, U>(this, second))
                : Optional<OptionalPair<T, U>>.Empty;
        }

        /// <summary>
        /// Creates an optional pair with the second instance.
        /// if one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public async Task<Optional<OptionalPair<T, U>>> AndAsync<U>(Func<Optional<T>, Task<Optional<U>>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = await right(this).ConfigureAwait(false);
            return second.Any()
                ? Optional<OptionalPair<T, U>>.Some(new OptionalPair<T, U>(this, second))
                : Optional<OptionalPair<T, U>>.Empty;
        }

        /// <summary>
        /// Creates an optional pair with the second instance.
        /// if one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public async Task<Optional<OptionalPair<T, U>>> AndAsync<U>(Func<Task<Optional<U>>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = await right().ConfigureAwait(false);
            return second.Any()
                ? Optional<OptionalPair<T, U>>.Some(new OptionalPair<T, U>(this, second))
                : Optional<OptionalPair<T, U>>.Empty;
        }

        /// <summary>
        /// Creates an optional pair with the second instance.
        /// if one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public async Task<Optional<OptionalPair<T, U>>> AndAsync<U>(Func<Task<U>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            Optional<U> second = await right().ConfigureAwait(false);
            return second.Any()
                ? Optional<OptionalPair<T, U>>.Some(new OptionalPair<T, U>(this, second))
                : Optional<OptionalPair<T, U>>.Empty;
        }

        /// <summary>
        /// Creates an optional pair with the second instance.
        /// if one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public async Task<Optional<OptionalPair<T, U>>> AndAsync<U>(Func<T, Task<U>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            Optional<U> second = await right(this).ConfigureAwait(false);
            return second.Any()
                ? Optional<OptionalPair<T, U>>.Some(new OptionalPair<T, U>(this, second))
                : Optional<OptionalPair<T, U>>.Empty;
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