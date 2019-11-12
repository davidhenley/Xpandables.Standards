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

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System
{
    public partial class Optional<T>
    {
        /// <summary>
        /// Creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> MapAsync([NotNull] Func<Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new optional that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="U"/>.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapAsync<U>([NotNull] Func<T, Task<U>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some(InternalValue).ConfigureAwait(false);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="U"/>.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapOptionalAsync<U>([NotNull] Func<T, Task<Optional<U>>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some(InternalValue).ConfigureAwait(false);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Applies the function to the element if the optional contains value.
        /// </summary>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task MapAsync([NotNull] Func<T, Task> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) await some(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function to the current instance.
        /// </summary>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Task MapOptionalAsync([NotNull] Func<Optional<T>, Task> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            return some(this);
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// Otherwise returns the current optional.
        /// </summary>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<T>> WhenAsync([NotNull] Predicate<T> predicate, [NotNull] Func<Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                return await some().ConfigureAwait(false);

            return this;
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// Otherwise returns the current optional.
        /// </summary>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<T>> WhenAsync([NotNull] Predicate<T> predicate, [NotNull] Func<T, Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                return await some(InternalValue).ConfigureAwait(false);

            return this;
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task WhenAsync([NotNull] Predicate<T> predicate, [NotNull] Func<T, Task> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                await some(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> WhenEmptyAsync([NotNull] Func<Task<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns an empty instance.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<U>> WhenEmptyAsync<U>([NotNull] Func<Task<U>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> WhenEmptyOptionalAsync([NotNull] Func<Task<Optional<T>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<U>> WhenEmptyOptionalAsync<U>([NotNull] Func<Task<Optional<U>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Applies the function if the optional is empty.
        /// </summary>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task WhenEmptyAsync([NotNull] Func<Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (!IsValue()) await action().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync([NotNull] Func<Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync([NotNull] Func<Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync([NotNull] Func<Exception, Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some(InternalException).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync([NotNull] Func<Exception, Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some(InternalException).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Applies the function only if the optional is exception.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task WhenExceptionAsync([NotNull] Func<Exception, Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException()) await action(InternalException).ConfigureAwait(false);
        }
    }
}