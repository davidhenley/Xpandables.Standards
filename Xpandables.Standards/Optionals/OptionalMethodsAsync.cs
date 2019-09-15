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
        public async Task<U> CastAsync<U>()
            => IsValue() && InternalValue is U target
                ? await Task.FromResult(target).ConfigureAwait(false)
                : default;

        /// <summary>
        /// Casts the element of optional to the specified type and returns an optional with the new value.
        /// if not, returns an empty optional of the target typed.
        /// </summary>
        /// <typeparam name="U">The type to cast to.</typeparam>
        public async Task<Optional<U>> CastOptionalAsync<U>() => await CastAsync<U>().ConfigureAwait(false);

        /// <summary>
        /// Returns the underlying value.
        /// If optional is empty, returns the default type of <typeparamref name="T"/>.
        /// <para>C#8.0 If <typeparamref name="T"/> is not nullable, be aware of <see cref="NullReferenceException"/>.</para>
        /// </summary>
        public async Task<T> ReturnAsync() => await Task.FromResult(InternalValue).ConfigureAwait(false);

        /// <summary>
        /// Returns the underlying exception.
        /// If optional does not contain an exception, returns the default type of exception.
        /// <para>C#8.0 If nullable is enable, be aware of <see cref="NullReferenceException"/>.</para>
        /// </summary>
        public async Task<Exception> ReturnExceptionAsync()
            => await Task.FromResult(InternalException).ConfigureAwait(false);

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The value to be used.</param>
        /// <returns>An optional pair.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public async Task<Optional<(T Left, U Right)>> AndAsync<U>(Task<U> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return IsValue() && !(await right.ConfigureAwait(false) == null)
                           ? (Optional<(T Left, U Right)>)(InternalValue, await right.ConfigureAwait(false))
                           : Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the optional is empty, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The optional to be used.</param>
        /// <returns>An optional pair.</returns>
        public async Task<Optional<(T Left, U Right)>> AndOptionalAsync<U>(Task<Optional<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return IsValue() && (await right.ConfigureAwait(false)).IsValue()
                           ? (Optional<(T Left, U Right)>)(InternalValue, (await right.ConfigureAwait(false)).InternalValue)
                           : Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional contains a value and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> MapAsync(Func<Task<T>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional contains a value and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapAsync<U>(Func<T, Task<U>> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return await some(InternalValue).ConfigureAwait(false);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional contains a value and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<U>> MapOptionalAsync<U>(Func<T, Task<Optional<U>>> some)
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
        /// Applies the function to the element.
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
        public async Task<Optional<(T Left, U Right)>> AndAsync<U>(Func<Task<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return (default, default);

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
        public async Task<Optional<(T Left, U Right)>> AndAsync<U>(Func<T, Task<U>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return (default, default);

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
        public async Task<Optional<(T Left, U Right)>> AndOptionalAsync<U>(Func<Task<Optional<U>>> right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return (default, default);

            return (await right().ConfigureAwait(false)).InternalValue is U result
                ? Optional<(T Left, U Right)>.Some((InternalValue, result))
                : (await right().ConfigureAwait(false)).InternalException is Exception exception
                    ? Optional<(T Left, U Right)>.Exception(exception)
                    : Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// Otherwise returns the current optional.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<T>> WhenAsync(Predicate<T> predicate, Func<Task<T>> some)
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
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public async Task<Optional<T>> WhenAsync(Predicate<T> predicate, Func<T, Task<T>> some)
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
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
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
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is empty and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceAsync(Func<Task<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return await empty().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is empty and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> ReduceOptionalAsync(Func<Task<Optional<T>>> empty)
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
        public async Task ReduceAsync(Func<Task> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (!IsValue()) await action().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is exception and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync(Func<Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is exception and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync(Func<Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is exception and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionAsync(Func<Exception, Task<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some(InternalException).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function to the element
        /// if the optional is exception and returns an optional with value or empty if the function returns null.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> WhenExceptionOptionalAsync(Func<Exception, Task<Optional<T>>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return await some(InternalException).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Applies the function to the given function to the element only if the optional is exception.
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