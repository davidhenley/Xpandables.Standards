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
using System.Threading;
using System.Threading.Tasks;

namespace System
{
#pragma warning disable HAA0303 // Lambda or anonymous method in a generic method allocates a delegate instance
    /// <summary>
    /// Option functional type.
    /// <para />The <see cref="Optional{T}"/> can contain a value of a type <typeparamref name="T"/> or no value.
    /// </summary>
    public partial class Optional<T>
    {
        /// <summary>
        /// Asynchronously filters optional of <typeparamref name="T"/> based on the specified <typeparamref name="U"/> type.
        /// If <typeparamref name="U"/> type is assignable from <typeparamref name="T"/> type,
        /// returns an optional of <typeparamref name="U"/>,
        /// otherwise returns empty of U.
        /// </summary>
        /// <typeparam name="U">The type to filter on.</typeparam>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public async Task<Optional<U>> OfTypeOptionalAsync<U>(CancellationToken cancellationToken = default)
            => await OfTypeAsync<U>(cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Asynchronously filters optional of <typeparamref name="T"/> based on the specified <typeparamref name="U"/> type.
        /// If <typeparamref name="U"/> type is assignable from <typeparamref name="T"/> type,
        /// returns an object of type <typeparamref name="U"/>,
        /// otherwise returns default U.
        /// </summary>
        /// <typeparam name="U">The type to filter on.</typeparam>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public async Task<U> OfTypeAsync<U>(CancellationToken cancellationToken = default)
            => this.Any() && this.Single() is U target ? await Task.FromResult(target).ConfigureAwait(false) : default;

        /// <summary>
        /// Asynchronously executes the specified delegate only if the current optional instance contains a value.
        /// </summary>
        /// <param name="some">The action to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public async Task MapAsync(Func<T, CancellationToken, Task> some, CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (this.Any()) await some(this.Single(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a pair optional with the second instance.
        /// </summary>
        /// <typeparam name="U">Type of the second instance.</typeparam>
        /// <param name="second">The instance to be added.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is null.</exception>
        public async Task<Optional<(Optional<T> First, Optional<U> Second)>> AndAsync<U>(
            Func<Optional<T>, CancellationToken, Task<Optional<U>>> second, CancellationToken cancellationToken = default)
        {
            if (second is null) throw new ArgumentNullException(nameof(second));

            return (this, await second(this, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Asynchronously executes the some delegate only if the current optional instance contains a value,
        /// otherwise executes the empty delegate.
        /// </summary>
        /// <param name="some">The delegate to be executed if optional contains a value.</param>
        /// <param name="empty">The delegate to be executed if optional is empty.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task MapAsync(
            Func<T, CancellationToken, Task> some, Func<T, CancellationToken, Task> empty, CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any())
                await some(this.Single(), cancellationToken).ConfigureAwait(false);
            else
                await empty(this.SingleOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value, otherwise the process returns empty of U.
        /// </summary>
        /// <typeparam name="U">The expected result type.</typeparam>
        /// <param name="some">The mapper which would be used in case that the optional contains value.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A value of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapAsync<U>(Func<T, CancellationToken, Task<U>> some, CancellationToken cancellationToken = default)
            => await MapAsync(some, (_, __) => default, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Asynchronously maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value,
        /// otherwise the empty delegate will be applied.
        /// </summary>
        /// <typeparam name="U">The expected type of result.</typeparam>
        /// <param name="some">The delegate which would be used in case that the optional contains value.</param>
        /// <param name="empty">The delegate which would be used if the optional contains no value.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<U>> MapAsync<U>(
            Func<T, CancellationToken, Task<U>> some, Func<T, CancellationToken, Task<U>> empty, CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any()
                ? await some(this.Single(), cancellationToken).ConfigureAwait(false)
                : await empty(this.SingleOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value, otherwise the process will return.
        /// </summary>
        /// <typeparam name="U">The expected type.</typeparam>
        /// <param name="some">The mapper which would be used in case that the optional contains a value.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional value of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapOptionalAsync<U>(
            Func<T, CancellationToken, Task<Optional<U>>> some, CancellationToken cancellationToken = default)
            => await MapOptionalAsync(some, (_, __) => Optional<U>.Empty()).ConfigureAwait(false);

        /// <summary>
        /// Asynchronously maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value,
        /// otherwise the empty delegate will be applied.
        /// </summary>
        /// <typeparam name="U">The expected type of result.</typeparam>
        /// <param name="some">The delegate which would be used in case that the optional contains value.</param>
        /// <param name="empty">The delegate which would be used if the optional contains no value.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<U>> MapOptionalAsync<U>(
            Func<T, CancellationToken, Task<Optional<U>>> some,
            Func<T, CancellationToken, Task<Optional<U>>> empty,
            CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any()
                ? await some(this.Single(), cancellationToken).ConfigureAwait(false)
                : await empty(this.SingleOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously filters the current optional instance applying the specified predicate.
        /// The predicate will be applied only if optional contains a value, otherwise the delegate replace value will be called.
        /// </summary>
        /// <param name="predicate">The predicate to be used in case that optional contains value.</param>
        /// <param name="replace">The delegate to be used if the current value doesn't match the predicate.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional value matching the predicate or the replace one.</returns>
        /// <exception cref="ArgumentException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="replace"/> is null.</exception>
        public async Task<Optional<T>> FilterAsync(
            Predicate<T> predicate, Func<CancellationToken, Task<T>> replace, CancellationToken cancellationToken = default)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (replace is null) throw new ArgumentNullException(nameof(replace));

            return this.Any() && predicate(this.Single()) ? this.Single() : await replace(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously filters the current optional instance applying the specified predicate.
        /// The predicate will be applied only if optional contains a value, otherwise the delegate replace value will be called.
        /// </summary>
        /// <param name="predicate">The predicate to be used in case that optional contains value.</param>
        /// <param name="replace">The delegate to be used if the current value doesn't match the predicate.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional value matching the predicate or the replace one.</returns>
        /// <exception cref="ArgumentException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="replace"/> is null.</exception>
        public async Task<Optional<T>> FilterOptionalAsync(
            Predicate<Optional<T>> predicate,
            Func<CancellationToken, Task<Optional<T>>> replace,
            CancellationToken cancellationToken = default)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (replace is null) throw new ArgumentNullException(nameof(replace));

            return this.Any() && predicate(this) ? this : await replace(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously transforms the value to optional according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The mapper which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<T>> WhenAsync(
            Predicate<T> predicate, Func<T, CancellationToken, Task<T>> some, CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (this.Any() && predicate(this.Single()))
                return await some(this.Single(), cancellationToken).ConfigureAwait(false);

            return this;
        }

        /// <summary>
        /// Asynchronously transforms the value to optional according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise applies the reduce delegate.
        /// </summary>
        /// <param name="some">The mapper which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The empty which would be used in case that the predicate is false.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<U>> WhenAsync<U>(
            Predicate<T> predicate,
            Func<T, CancellationToken, Task<U>> some,
            Func<T, CancellationToken, Task<U>> empty,
            CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any() && predicate(this.Single()))
                return await some(this.Single(), cancellationToken).ConfigureAwait(false);

            return await empty(this.SingleOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously calls the action according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value.
        /// </summary>
        /// <param name="some">The action which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task WhenAsync(Predicate<T> predicate, Func<T, CancellationToken, Task> some, CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single()))
                await some(this.Single(), cancellationToken).ConfigureAwait(false);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously calls the action according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise calls the empty action.
        /// </summary>
        /// <param name="some">The action which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The empty which would be used in case that the predicate is false.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task WhenAsync(
            Predicate<T> predicate,
            Func<T, CancellationToken, Task> some,
            Func<T, CancellationToken, Task> empty,
            CancellationToken cancellationToken = default)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any() && predicate(this.Single()))
                await some(this.Single(), cancellationToken).ConfigureAwait(false);
            else
                await empty(this.SingleOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method would have to receive a delegate that produces the replacement value,
        /// which would be used in case that the optional contains no value.
        /// </summary>
        /// <param name="empty">The delegate to produce the replacement value.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceAsync(
            Func<CancellationToken, Task<T>> empty, CancellationToken cancellationToken = default)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? this.Single() : await empty(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method would have to receive a delegate that produces the replacement optional,
        /// which would be used in case that the current optional contains no value.
        /// </summary>
        /// <param name="empty">The delegate to produce the replacement value option.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceOptionalAsync(
            Func<CancellationToken, Task<Optional<T>>> empty, CancellationToken cancellationToken = default)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? this : await empty(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the specified delegate only if the current optional instance contains no value.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public async Task ReduceAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            if (!this.Any()) await action(cancellationToken).ConfigureAwait(false);
        }
    }
}