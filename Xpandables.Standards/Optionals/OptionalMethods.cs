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
        /// <typeparam name="U">The type to cast the elements of source to.</typeparam>
        public Optional<U> CastOptional<U>() => Cast<U>();

        /// <summary>
        /// Casts the element of optional to the specied type.
        /// </summary>
        /// <typeparam name="U">The type to cast the elements of source to.</typeparam>
        public U Cast<U>() => this.Any() && this.Single() is U target ? target : default;

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<U> Map<U>(Func<T, U> some, Func<T, U> empty)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? some(this.Single()) : empty(this.SingleOrDefault());
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="empty">The empty map.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public void Map(Action<T> some, Action<T> empty)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any())
                some(this.Single());
            else
                empty(this.SingleOrDefault());
        }

        /// <summary>
        /// Creates a pair optional pair with the second instance.
        /// </summary>
        /// <typeparam name="U">The type of the second instance</typeparam>
        /// <param name="second">The instance to be added.</param>
        /// <returns>An optional of pair instance of optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is null.</exception>
        public Optional<(Optional<T> First, Optional<U> Second)> And<U>(Func<Optional<T>, Optional<U>> second)
        {
            if (second is null) throw new ArgumentNullException(nameof(second));
            return (this, second(this));
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element.
        /// </summary>
        /// <typeparam name="U">The expected type of result.</typeparam>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>An optional of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<U> MapOptional<U>(Func<T, Optional<U>> some, Func<T, Optional<U>> empty)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? some(this.Single()) : empty(this.SingleOrDefault());
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element when predicate is true.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<U> When<U>(Predicate<T> predicate, Func<T, U> some, Func<T, U> empty)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any() && predicate(this.Single()))
                return some(this.Single());

            return empty(this.SingleOrDefault());
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element when predicate is true.
        /// </summary>
        /// <param name="some">The function to transform the element.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public void When(Predicate<T> predicate, Action<T> some, Action<T> empty)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            if (this.Any() && predicate(this.Single()))
                some(this.Single());
            else
                empty(this.SingleOrDefault());
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
    }
}