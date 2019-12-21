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
        public async Task<Optional<T>> MapAsync(Func<Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            return IsValue() ? await some().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new optional that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TU"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<TU>> MapAsync<TU>(Func<T, Task<TU>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some(InternalValue).ConfigureAwait(false);
            return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TU"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<TU>> MapOptionalAsync<TU>(Func<T, Task<Optional<TU>>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some(InternalValue).ConfigureAwait(false);
            return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
        }

        /// <summary>
        /// Applies the function to the element if the optional contains value.
        /// </summary>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task MapAsync(Func<T, Task> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) await some(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function to the current instance.
        /// </summary>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task MapOptionalAsync(Func<Optional<T>, Task> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            await some(this).ConfigureAwait(false);
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
        public async Task<Optional<T>> WhenAsync(Predicate<T> predicate, Func<Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return IsValue() && predicate(InternalValue) ? await some().ConfigureAwait(false) : this;
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
        public async Task<Optional<T>> WhenAsync(Predicate<T> predicate, Func<T, Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return IsValue() && predicate(InternalValue)
                ? await some(InternalValue).ConfigureAwait(false)
                : this;
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task WhenAsync(Predicate<T> predicate, Func<T, Task> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                await some(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// When the optional contains a value, applies the <paramref name="trueAction"/> to the element if the value matches the predicate,
        /// otherwise applies the <paramref name="falseAction"/>.
        /// If the optional contains no value, returns an empty optional of <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="trueAction">The delegate to be executed on true predicate with value.</param>
        /// <param name="falseAction">The delegate to be executed</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="trueAction"/> is null</exception>
        /// /// <exception cref="ArgumentNullException">The <paramref name="falseAction"/> is null</exception>
        public async Task<Optional<TU>> WhenAsync<TU>(Predicate<T> predicate, Func<T, Task<TU>> trueAction, Func<T, Task<TU>> falseAction)
        {
            if (trueAction is null) throw new ArgumentNullException(nameof(trueAction));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (falseAction is null) throw new ArgumentNullException(nameof(falseAction));

            if (!IsValue()) return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
            if (predicate(InternalValue))
                return await trueAction(InternalValue).ConfigureAwait(false);

            return await falseAction(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> WhenEmptyAsync(Func<Task<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return !IsValue() ? await empty().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns an empty instance.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<TU>> WhenEmptyAsync<TU>(Func<Task<TU>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> WhenEmptyOptionalAsync(Func<Task<Optional<T>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return !IsValue() ? await empty().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<TU>> WhenEmptyOptionalAsync<TU>(Func<Task<Optional<TU>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
        }

        /// <summary>
        /// Applies the function if the optional is empty.
        /// </summary>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task WhenEmptyAsync(Func<Task> action)
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
        public async Task<Optional<T>> WhenExceptionAsync(Func<Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() ? await some().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specified type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync<TException>(Func<Task<T>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException ? await some().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync(Func<Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() ? await some().ConfigureAwait(false) : (this);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specified type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync<TException>(Func<Task<Optional<T>>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException ? await some().ConfigureAwait(false) : (this);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync(Func<Exception, Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() ? await some(InternalException).ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specified type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync<TException>(Func<TException, Task<T>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException exception
                ? await some(exception).ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync(Func<Exception, Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() ? await some(InternalException).ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specified type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync<TException>(Func<TException, Task<Optional<T>>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException exception
                ? await some(exception).ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Applies the function only if the optional is exception.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task WhenExceptionAsync(Func<Exception, Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException()) await action(InternalException).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function only if the optional is exception is of the specified type.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <typeparam name="TException">the type of exception.</typeparam>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public async Task WhenExceptionAsync<TException>(Func<TException, Task> action)
            where TException : Exception
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException() && InternalException is TException exception) await action(exception).ConfigureAwait(false);
        }
    }
}