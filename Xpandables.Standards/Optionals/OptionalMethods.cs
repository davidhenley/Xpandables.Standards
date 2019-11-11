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
using System.Diagnostics.CodeAnalysis;

namespace System
{
#nullable disable
    public partial class Optional<T>
    {
        /// <summary>
        /// Casts the value of optional to the specified type and returns the new value.
        /// if not, returns the default type of value.
        /// </summary>
        /// <typeparam name="U">The type to cast the value to.</typeparam>
        public U Cast<U>() => IsValue() && InternalValue is U target ? target : default;

        /// <summary>
        /// Casts the element of optional to the specified type and returns an optional with the new value.
        /// if not, returns an empty optional of the target typed.
        /// </summary>
        /// <typeparam name="U">The type to cast to.</typeparam>
        public Optional<U> CastOptional<U>() => Cast<U>();

        /// <summary>
        /// Converts the current instance to an empty one.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public Optional<T> ToEmpty() => Empty();

        /// <summary>
        /// Converts the current instance to an empty of the specific type.
        /// </summary>
        /// <typeparam name="U">The type to store.</typeparam>
        /// <returns>An empty optional of the specific type.</returns>
        public Optional<U> ToEmpty<U>() => Optional<U>.Empty();

        /// <summary>
        /// Converts the current instance to an optional with the specified value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public Optional<T> ToSome([NotNull] T value) => Some(value);

        /// <summary>
        /// Converts the current instance to an optional of the specified type with value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public Optional<U> ToSome<U>([NotNull] U value) => Optional<U>.Some(value);

        /// <summary>
        /// Converts the current instance to an optional with exception.
        /// </summary>
        /// <param name="exception">The exception to be used.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public Optional<T> ToException([NotNull] Exception exception) => Exception(exception);

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the value is null, returns an empty optional.
        /// If the left is an exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The value to be used.</param>
        /// <returns>An optional pair.</returns>
        public Optional<(T Left, U Right)> And<U>(U right)
            => IsValue() && !EqualityComparer<U>.Default.Equals(right, default)
                ? (Optional<(T Left, U Right)>)(InternalValue, right)
                : IsException()
                    ? Optional<(T Left, U Right)>.Exception(InternalException)
                    : Optional<(T Left, U Right)>.Empty();

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the optional is empty, returns an empty optional.
        /// If one of the optional is exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="U">The type of the right side.</typeparam>
        /// <param name="right">The optional to be used.</param>
        /// <returns>An optional pair.</returns>
        public Optional<(T Left, U Right)> AndOptional<U>(Optional<U> right)
        {
            if (!(right is null))
            {
                if (IsValue() && right.IsValue())
                    return Optional<(T Left, U Right)>.Some((InternalValue, right.InternalValue));

                if (IsException() && !right.IsException())
                    return Optional<(T Left, U Right)>.Exception(InternalException);

                if (IsException() && right.IsException())
                    return Optional<(T Left, U Right)>.Exception(new AggregateException(InternalException, right.InternalException));

                if (!IsException() && right.IsException())
                    return Optional<(T Left, U Right)>.Exception(right.InternalException);
            }

            return Optional<(T Left, U Right)>.Empty();
        }

        /// <summary>
        /// Creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> Map(Func<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return some();
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
        public Optional<U> Map<U>(Func<T, U> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return some(InternalValue);
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
        public Optional<U> MapOptional<U>(Func<T, Optional<U>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return some(InternalValue);
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Applies the function to the element only if the optional contains value.
        /// </summary>
        /// <param name="some">The function to apply to the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void Map(Action<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) some(InternalValue);
        }

        /// <summary>
        /// Applies the function to the current instance.
        /// </summary>
        /// <param name="some">The function to apply to the instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void MapOptional(Action<Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            some(this);
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public Optional<(T Left, U Right)> And<U>(Func<U> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return Optional<(T Left, U Right)>.Empty();
            if (IsException()) return Optional<(T Left, U Right)>.Exception(InternalException);

            return right() is U result
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
        public Optional<(T Left, U Right)> And<U>(Func<T, U> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return Optional<(T Left, U Right)>.Empty();
            if (IsException()) return Optional<(T Left, U Right)>.Exception(InternalException);

            return right(InternalValue) is U result
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
        public Optional<(T Left, U Right)> AndOptional<U>(Func<Optional<U>> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));
            return AndOptional(right());
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
        public Optional<T> When(Predicate<T> predicate, Func<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                return some();

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
        public Optional<T> When(Predicate<T> predicate, Func<T, T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                return some(InternalValue);

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
        public void When(Predicate<T> predicate, Action<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (IsValue() && predicate(InternalValue))
                some(InternalValue);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> WhenEmpty(Func<T> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return empty();
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
        public Optional<U> WhenEmpty<U>(Func<U> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return empty();
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns an empty instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> WhenEmptyOptional(Func<Optional<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return empty();
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
        public Optional<U> WhenEmptyOptional<U>(Func<Optional<U>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return empty();
            return IsException() ? Optional<U>.Exception(InternalException) : Optional<U>.Empty();
        }

        /// <summary>
        /// Applies the function if the optional is empty.
        /// </summary>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public void WhenEmpty(Action action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (!IsValue()) action();
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException(Func<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return some();
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public Optional<T> WhenExceptionOptional(Func<Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return some();
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException(Func<Exception, T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return some(InternalException);
            return this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenExceptionOptional(Func<Exception, Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsException()) return some(InternalException);
            return this;
        }

        /// <summary>
        /// Applies the function only if the optional is exception.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public void WhenException(Action<Exception> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException()) action(InternalException);
        }
    }
#nullable enable
}