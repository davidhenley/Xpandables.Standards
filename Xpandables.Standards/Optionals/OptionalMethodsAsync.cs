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
        /// Casts the value of optional to the specified type and returns the new value.
        /// if not, returns the default type of value.
        /// </summary>
        /// <typeparam name="U">The type to cast the value to.</typeparam>
        public async ValueTask<U> CastAsync<U>() => await Task.FromResult(Cast<U>()).ConfigureAwait(false);

        /// <summary>
        /// Casts the element of optional to the specified type and returns an optional with the new value.
        /// if not, returns an empty optional of the target typed.
        /// </summary>
        /// <typeparam name="U">The type to cast to.</typeparam>
        public async ValueTask<Optional<U>> CastOptionalAsync<U>() => await CastAsync<U>().ConfigureAwait(false);

        /// <summary>
        /// Converts the current instance to an empty one.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public async ValueTask<Optional<T>> ToEmptyAsync() => await Task.FromResult(Empty()).ConfigureAwait(false);

        /// <summary>
        /// Converts the current instance to an empty of the specific type.
        /// </summary>
        /// <typeparam name="U">The type to store.</typeparam>
        /// <returns>An empty optional of the specific type.</returns>
        public async ValueTask<Optional<U>> ToEmptyAsync<U>() => await Task.FromResult(Optional<U>.Empty()).ConfigureAwait(false);

        /// <summary>
        /// Converts the current instance to an optional with the specified value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public async ValueTask<Optional<T>> ToSomeAsync(T value) => await Task.FromResult(Some(value)).ConfigureAwait(false);

        /// <summary>
        /// Converts the current instance to an optional of the specified type with value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public async ValueTask<Optional<U>> ToSomeAsync<U>(U value)
            => await Task.FromResult(Optional<U>.Some(value)).ConfigureAwait(false);

        /// <summary>
        /// Converts the current instance to an optional with exception.
        /// </summary>
        /// <param name="exception">The exception to be used.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public async ValueTask<Optional<T>> ToExceptionAsync(Exception exception)
            => await Task.FromResult(Exception(exception)).ConfigureAwait(false);

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the value is null, returns an empty optional.
        /// If the left is an exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The value to be used.</param>
        /// <returns>An optional pair.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public async ValueTask<Optional<(T Left, U Right)>> AndAsync<U>(ValueTask<U> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            var result = await right.ConfigureAwait(false);
            return And(result);
        }

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the optional is empty, returns an empty optional.
        /// If one of the optional is exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The optional to be used.</param>
        /// <returns>An optional pair.</returns>
        public async ValueTask<Optional<(T Left, U Right)>> AndOptionalAsync<U>(ValueTask<Optional<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            var result = await right.ConfigureAwait(false);
            return AndOptional(result);
        }

        /// <summary>
        /// Creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async ValueTask<Optional<T>> MapAsync(Func<ValueTask<T>> some)
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
        public async ValueTask<Optional<U>> MapAsync<U>(Func<T, ValueTask<U>> some)
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
        public async ValueTask<Optional<U>> MapOptionalAsync<U>(Func<T, ValueTask<Optional<U>>> some)
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
        public Task MapOptionalAsync(Func<Optional<T>, Task> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            return some(this);
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public async ValueTask<Optional<(T Left, U Right)>> AndAsync<U>(Func<ValueTask<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return Optional<(T Left, U Right)>.Empty();
            if (IsException()) return Optional<(T Left, U Right)>.Exception(InternalException);

            return (await right().ConfigureAwait(false)) is U result
                ? Optional<(T Left, U Right)>.Some((InternalValue, result))
                : Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public async ValueTask<Optional<(T Left, U Right)>> AndAsync<U>(Func<T, ValueTask<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return Optional<(T Left, U Right)>.Empty();
            if (IsException()) return Optional<(T Left, U Right)>.Exception(InternalException);

            return (await right(InternalValue).ConfigureAwait(false)) is U result
                ? Optional<(T Left, U Right)>.Some((InternalValue, result))
                : Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public ValueTask<Optional<(T Left, U Right)>> AndOptionalAsync<U>(Func<ValueTask<Optional<U>>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return AndOptionalAsync(right());
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
        public async ValueTask<Optional<T>> WhenAsync(Predicate<T> predicate, Func<ValueTask<T>> some)
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
        public async ValueTask<Optional<T>> WhenAsync(Predicate<T> predicate, Func<T, ValueTask<T>> some)
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
        public async Task WhenAsync(Predicate<T> predicate, Func<T, Task> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                await some(InternalValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance contains is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async ValueTask<Optional<T>> WhenEmptyAsync(Func<ValueTask<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance contains is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async ValueTask<Optional<T>> WhenEmptyOptionalAsync(Func<ValueTask<Optional<T>>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return this;
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
        public async ValueTask<Optional<T>> WhenExceptionAsync(Func<ValueTask<T>> some)
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
        public async ValueTask<Optional<T>> WhenExceptionOptionalAsync(Func<ValueTask<Optional<T>>> some)
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
        public async ValueTask<Optional<T>> WhenExceptionAsync(Func<Exception, ValueTask<T>> some)
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
        public async ValueTask<Optional<T>> WhenExceptionOptionalAsync(Func<Exception, ValueTask<Optional<T>>> some)
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
        public async Task WhenExceptionAsync(Func<Exception, Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException()) await action(InternalException).ConfigureAwait(false);
        }
    }
}