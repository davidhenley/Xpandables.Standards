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

namespace System
{
    public partial class Optional<T>
    {
        /// <summary>
        /// Casts the element of optional to the specied type.
        /// </summary>
        /// <typeparam name="TResult">The type to cast the elements of source to.</typeparam>
        public Optional<TResult> CastOptional<TResult>() => Cast<TResult>();

        /// <summary>
        /// Converts the optional to an empty one.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public Optional<T> ToEmpty() => Empty;

        /// <summary>
        /// Converts the optional to an empty of the specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>An empty optional of the specific type.</returns>
        public Optional<TResult> ToEmpty<TResult>() => Optional<TResult>.Empty;

        /// <summary>
        /// Converts the optional to optional pair.
        /// </summary>
        /// <typeparam name="U">The type of the right.</typeparam>
        /// <param name="right">The value to used.</param>
        /// <returns>An optional pair.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1715:Les identificateurs doivent être dotés d'un préfixe correct", Justification = "<En attente>")]
        public Optional<OptionalPair<T, U>> ToOptionalPair<U>(U right) => new OptionalPair<T, U>(this, right);

        /// <summary>
        /// Converts the optional to an optional with exception.
        /// </summary>
        /// <param name="exception">The exception ot be used.</param>
        /// <returns>An optional with exception value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public Optional<T> ToException(Exception exception) => Optional<T>.Exception(exception);

        /// <summary>
        /// Returns the underlying value.
        /// If optional is empty, returns the default type of <typeparamref name="T"/>.
        /// <para>if <typeparamref name="T"/> is not nullable, be aware of exception.</para>
        /// </summary>
        public T Return() => Cast<T>();

        /// <summary>
        /// Casts the element of optional to the specied type.
        /// </summary>
        /// <typeparam name="TResult">The type to cast the elements of source to.</typeparam>
        public TResult Cast<TResult>() => this.Any() && this.Single() is TResult result ? result : default;

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<TResult> Map<TResult>(Func<T, TResult> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return this.Any() ? some(this.Single()) : default;
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void Map(Action<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (this.Any()) some(this.Single());
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
        public Optional<OptionalPair<T, U>> And<U>(Func<Optional<T>, Optional<U>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = right(this);
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
        public Optional<OptionalPair<T, U>> And<U>(Func<T, Optional<U>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = right(this);
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
        public Optional<OptionalPair<T, U>> And<U>(Func<Optional<U>> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = right();
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
        public Optional<OptionalPair<T, U>> And<U>(Func<U> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = right().ToOptional();
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
        public Optional<OptionalPair<T, U>> And<U>(Func<T, U> right)
        {
            if (!this.Any()) return Optional<OptionalPair<T, U>>.Empty;
            if (right is null) throw new ArgumentNullException(nameof(right));

            var second = right(this).ToOptional();
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
        public Optional<TResult> MapOptional<TResult>(Func<T, Optional<TResult>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return this.Any() ? some(this.Single()) : Optional<TResult>.Empty;
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element when predicate is true.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public Optional<TResult> When<TResult>(Predicate<T> predicate, Func<T, TResult> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single()))
                return some(this.Single());

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
        public void When(Predicate<T> predicate, Action<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single())) some(this.Single());
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> Reduce(Func<T> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return this.Any() ? this.Single() : empty();
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> ReduceOptional(Func<Optional<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));
            return this.Any() ? this : empty();
        }

        /// <summary>
        /// Creates a new element that is the result of applying the empty function to the element.
        /// </summary>
        /// <param name="action">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public void Reduce(Action action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (!this.Any()) action();
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element on exception.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException(Func<Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return _exceptions.Any() ? some() : (this);
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element on exception.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional with value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> WhenException(Func<Exception, Optional<T>> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            return _exceptions.Any() ? some(_exceptions.Single()) : this;
        }

        /// <summary>
        /// Executes the given delegate on exception.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public void WhenException(Action<Exception> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (_exceptions.Any()) action(_exceptions.Single());
        }
    }
}