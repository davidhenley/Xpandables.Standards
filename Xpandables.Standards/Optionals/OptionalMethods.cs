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

namespace System
{
    public partial class Optional<T>
    {
        /// <summary>
        /// Converts the current instance to an empty one.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public Optional<T> ToEmpty() => OptionalBuilder.Empty<T>();

        /// <summary>
        /// Converts the current instance to an empty of another type.
        /// </summary>
        /// <typeparam name="TU">The type to store.</typeparam>
        /// <returns>An empty optional of the specific type.</returns>
        public Optional<TU> ToEmpty<TU>() => OptionalBuilder.Empty<TU>();

        /// <summary>
        /// Converts the current instance to the specific type if possible.
        /// Otherwise returns an empty optional if the current is empty or an exception optional if the current is an exception.
        /// </summary>
        /// <typeparam name="TU">The type to convert to.</typeparam>
        public Optional<TU> ToOptional<TU>()
        {
            if (IsValue() && InternalValue is TU target) return target;
            return IsException() ? OptionalBuilder.Exception<TU>(InternalException) : OptionalBuilder.Empty<TU>();
        }

        /// <summary>
        /// Converts the current instance to an optional with the specified value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public Optional<T> ToSome(T value) => value.AsOptional();

        /// <summary>
        /// Converts the current instance to an optional of the specified type with value.
        /// </summary>
        /// <typeparam name="TU">The specific type of optional.</typeparam>
        /// <param name="value">The value to be used.</param>
        /// <returns>An optional that contains a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public Optional<TU> ToSome<TU>(TU value) => OptionalBuilder.Some(value);

        /// <summary>
        /// Converts the current instance to an optional with exception.
        /// </summary>
        /// <param name="exception">The exception to be used.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public Optional<T> ToException(Exception exception) => exception.AsOptional<T>();

        /// <summary>
        /// Converts the current instance to an optional of specific type with exception.
        /// </summary>
        /// <typeparam name="TU">The specific type of optional.</typeparam>
        /// <param name="exception">The exception to be used.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public Optional<TU> ToException<TU>(Exception exception) => exception.AsOptional<TU>();

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the value is null, returns an empty optional.
        /// If the left is an exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="TU">The type of the right side.</typeparam>
        /// <param name="right">The value to be used.</param>
        /// <returns>An optional pair.</returns>
        public Optional<(T Left, TU Right)> And<TU>(TU right)
            => IsValue() && !(right is null)
                ? (InternalValue, right)
                : IsException()
                    ? InternalException.AsOptional<(T Left, TU Right)>()
                    : OptionalBuilder.Empty<(T Left, TU Right)>();

        /// <summary>
        /// Converts the optional to optional pair.
        /// If one of the optional is empty, returns an empty optional.
        /// If one of the optional is exception, returns an optional with exception.
        /// </summary>
        /// <typeparam name="TU">The type of the right side.</typeparam>
        /// <param name="right">The optional to be used.</param>
        /// <returns>An optional pair.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public Optional<(T Left, TU Right)> AndOptional<TU>(Optional<TU> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));

            if (IsValue() && right.IsValue())
                return OptionalBuilder.Some((InternalValue, right.InternalValue));

            if (IsException() && !right.IsException())
                return OptionalBuilder.Exception<(T Left, TU Right)>(InternalException);

            if (IsException() && right.IsException())
            {
                return OptionalBuilder.Exception<(T Left, TU Right)>(
                    new AggregateException(InternalException, right.InternalException));
            }

            if (!IsException() && right.IsException())
                return OptionalBuilder.Exception<(T Left, TU Right)>(right.InternalException);

            return OptionalBuilder.Empty<(T Left, TU Right)>();
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
        /// otherwise returns an empty optional of <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TU"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<TU> Map<TU>(Func<T, TU> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return some(InternalValue);
            return IsException() ? InternalException.AsOptional<TU>() : OptionalBuilder.Empty<TU>();
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
        public Optional<TU> MapOptional<TU>(Func<T, Optional<TU>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (IsValue()) return some(InternalValue);
            return IsException() ? InternalException.AsOptional<TU>() : OptionalBuilder.Empty<TU>();
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
        /// <typeparam name="TU">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public Optional<(T Left, TU Right)> And<TU>(Func<TU> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return ToEmpty<(T Left, TU Right)>();
            if (IsException()) return ToException<(T Left, TU Right)>(InternalException);

            return right() is { } result
                ? ToSome<(T Left, TU Right)>((InternalValue, result))
                : ToEmpty<(T Left, TU Right)>();
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="TU">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public Optional<(T Left, TU Right)> And<TU>(Func<T, TU> right)
        {
            if (right is null) throw new ArgumentNullException(nameof(right));
            if (!IsValue()) return ToEmpty<(T Left, TU Right)>();
            if (IsException()) return ToException<(T Left, TU Right)>(InternalException);

            return right(InternalValue) is { } result
                ? ToSome<(T Left, TU Right)>((InternalValue, result))
                : ToEmpty<(T Left, TU Right)>();
        }

        /// <summary>
        /// Converts the optional to optional pair with the second value from function.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="TU">The type of the second instance</typeparam>
        /// <param name="right">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public Optional<(T Left, TU Right)> AndOptional<TU>(Func<Optional<TU>> right)
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

            return IsValue() && predicate(InternalValue) ? some() : this;
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

            return IsValue() && predicate(InternalValue) ? some(InternalValue) : this;
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
        public Optional<TU> When<TU>(Predicate<T> predicate, Func<T, TU> trueAction, Func<T, TU> falseAction)
        {
            if (trueAction is null) throw new ArgumentNullException(nameof(trueAction));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (falseAction is null) throw new ArgumentNullException(nameof(falseAction));

            if (!IsValue()) return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
            return predicate(InternalValue) ? trueAction(InternalValue) : falseAction(InternalValue);
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
            return !IsValue() ? empty() : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns an empty instance.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<TU> WhenEmpty<TU>(Func<TU> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return !IsValue() ? empty() : IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
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
            return !IsValue() ? empty() : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<TU> WhenEmptyOptional<TU>(Func<Optional<TU>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            if (!IsValue()) return empty();
            return IsException() ? ToException<TU>(InternalException) : ToEmpty<TU>();
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
            return IsException() ? some() : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specific type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException<TException>(Func<T> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException ? some() : this;
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
            return IsException() ? some() : (this);
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specific type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public Optional<T> WhenExceptionOptional<TException>(Func<Optional<T>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException ? some() : (this);
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
            return IsException() ? some(InternalException) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception is of the specific type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException<TException>(Func<TException, T> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException exception ? some(exception) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when exception.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenExceptionOptional(Func<Exception, Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() ? some(InternalException) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the exception is of the specific type.
        /// The delegate get called only if the instance is an exception, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="TException">the type of exception.</typeparam>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenExceptionOptional<TException>(Func<TException, Optional<T>> some)
            where TException : Exception
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return IsException() && InternalException is TException exception ? some(exception) : this;
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

        /// <summary>
        /// Applies the function only if the optional is exception of the specific type.
        /// The delegate get called only if the instance is an exception.
        /// </summary>
        /// <typeparam name="TException">The type of exception.</typeparam>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public void WhenException<TException>(Action<TException> action)
            where TException : Exception
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (IsException() && InternalException is TException exception) action(exception);
        }
    }
}