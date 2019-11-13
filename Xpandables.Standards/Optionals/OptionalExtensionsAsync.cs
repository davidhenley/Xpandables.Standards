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
        public static async Task<Optional<T>> WhenAsync<T>(this T source, bool predicate)
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
        public static async Task<Optional<T>> WhenAsync<T>(this T source, Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return predicate(source)
                ? await Task.FromResult(source.AsOptional()).ConfigureAwait(false)
                : Optional<T>.Empty();
        }

        /// <summary>
        /// Creates a new optional that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="U"/>.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public static async Task<Optional<U>> MapAsync<T, U>(this Task<Optional<T>> optional, Func<T, Task<U>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new optional that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public static async Task MapAsync<T>(this Task<Optional<T>> optional, Func<T, Task> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new optional that is the result of applying the given function to the element.
        /// The delegate get called only if the instance contains a value,
        /// otherwise returns an empty optional of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public static async Task<Optional<U>> MapOptionalAsync<T, U>(this Task<Optional<T>> optional, Func<T, Task<Optional<U>>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).MapOptionalAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// Otherwise returns the current optional.
        /// </summary>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public static async Task<Optional<T>> WhenAsync<T>(this Task<Optional<T>> optional, Predicate<T> predicate, Func<T, Task<T>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains a value and matches the predicate.
        /// </summary>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public static async Task WhenAsync<T>(this Task<Optional<T>> optional, Predicate<T> predicate, Func<T, Task> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            await (await optional.ConfigureAwait(false)).WhenAsync(predicate, some).ConfigureAwait(false);
        }

        /// <summary>
        /// When the optional contains a value, applies the <paramref name="trueAction"/> to the element if the value matches the predicate,
        /// otherwise applies the <paramref name="falseAction"/>.
        /// If the optional contains no value, returns an empty optional of <typeparamref name="U"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="trueAction">The delegate to be executed on true predicate with value.</param>
        /// <param name="falseAction">The delegate to be executed</param>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="trueAction"/> is null</exception>
        /// /// <exception cref="ArgumentNullException">The <paramref name="falseAction"/> is null</exception>
        public static async Task<Optional<U>> WhenAsync<T, U>(
            this Task<Optional<T>> optional, [NotNull] Predicate<T> predicate, Func<T, Task<U>> trueAction, Func<T, Task<U>> falseAction)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (trueAction is null) throw new ArgumentNullException(nameof(trueAction));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (falseAction is null) throw new ArgumentNullException(nameof(falseAction));
            return await (await optional.ConfigureAwait(false)).WhenAsync(predicate, trueAction, falseAction).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public static async Task<Optional<T>> WhenEmptyAsync<T>(this Task<Optional<T>> optional, Func<Task<T>> empty)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns an empty instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public static async Task<Optional<U>> WhenEmptyAsync<T, U>(this Task<Optional<T>> optional, Func<Task<U>> empty)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public static async Task<Optional<T>> WhenEmptyOptionalAsync<T>(this Task<Optional<T>> optional, Func<Task<Optional<T>>> empty)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return await (await optional.ConfigureAwait(false)).WhenEmptyOptionalAsync(empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public static async Task<Optional<U>> WhenEmptyOptionalAsync<T, U>(this Task<Optional<T>> optional, Func<Task<Optional<U>>> empty)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return await (await optional.ConfigureAwait(false)).WhenEmptyOptionalAsync(empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function if the optional is empty.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public static async Task WhenEmptyAsync<T>(this Task<Optional<T>> optional, Func<Task> action)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (action is null) throw new ArgumentNullException(nameof(action));
            await (await optional.ConfigureAwait(false)).WhenEmptyAsync(action).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public static async Task<Optional<T>> WhenExceptionAsync<T>(this Task<Optional<T>> optional, Func<Task<T>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public static async Task<Optional<T>> WhenExceptionOptionalAsync<T>(this Task<Optional<T>> optional, Func<Task<Optional<T>>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public static async Task<Optional<T>> WhenExceptionAsync<T>(this Task<Optional<T>> optional, Func<Exception, Task<T>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public static async Task<Optional<T>> WhenExceptionOptionalAsync<T>(this Task<Optional<T>> optional, Func<Exception, Task<Optional<T>>> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            return await (await optional.ConfigureAwait(false)).WhenExceptionOptionalAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the function only if the optional is exception.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to return the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="optional"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public static async Task WhenExceptionAsync<T>(this Task<Optional<T>> optional, Func<Exception, Task> some)
        {
            if (optional is null) throw new ArgumentNullException(nameof(optional));
            if (some is null) throw new ArgumentNullException(nameof(some));
            await (await optional.ConfigureAwait(false)).WhenExceptionAsync(some).ConfigureAwait(false);
        }
    }
}
