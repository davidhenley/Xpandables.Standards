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
        /// Filters optional of <typeparamref name="T"/> based on the specified <typeparamref name="U"/> type.
        /// If <typeparamref name="U"/> type is assignable from <typeparamref name="T"/> type,
        /// returns an optional of <typeparamref name="U"/>,
        /// otherwise returns empty of U.
        /// </summary>
        /// <typeparam name="U">The type to filter on.</typeparam>
        public Optional<U> OfTypeOptional<U>() => OfType<U>();

        /// <summary>
        /// Filters optional of <typeparamref name="T"/> based on the specified <typeparamref name="U"/> type.
        /// If <typeparamref name="U"/> type is assignable from <typeparamref name="T"/> type,
        /// returns an object of type <typeparamref name="U"/>,
        /// otherwise returns default U.
        /// </summary>
        /// <typeparam name="U">The type to filter on.</typeparam>
        public U OfType<U>() => this.Any() && this.Single() is U target ? target : default;

        /// <summary>
        /// Executes the specified delegate only if the current optional instance contains a value.
        /// </summary>
        /// <param name="some">The action to be executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void Map(Action<T> some) => Map(some, _ => { });

        /// <summary>
        /// Executes the some delegate only if the current optional instance contains a value,
        /// otherwise executes the empty delegate.
        /// </summary>
        /// <param name="some">The delegate to be executed if optional contains a value.</param>
        /// <param name="empty">The delegate to be executed if optional is empty.</param>
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
        /// Maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value, otherwise the process returns empty of U.
        /// </summary>
        /// <typeparam name="U">The expected result type.</typeparam>
        /// <param name="some">The mapper which would be used in case that the optional contains value.</param>
        /// <returns>A value of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<U> Map<U>(Func<T, U> some) => Map(some, _ => default);

        /// <summary>
        /// Maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value,
        /// otherwise the empty delegate will be applied.
        /// </summary>
        /// <typeparam name="U">The expected type of result.</typeparam>
        /// <param name="some">The delegate which would be used in case that the optional contains value.</param>
        /// <param name="empty">The delegate which would be used if the optional contains no value.</param>
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
        /// Maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value, otherwise the process will return an empty optional.
        /// </summary>
        /// <typeparam name="U">The expected type.</typeparam>
        /// <param name="some">The mapper which would be used in case that the optional contains a value.</param>
        /// <returns>An optional value of <typeparamref name="U"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<U> MapOptional<U>(Func<T, Optional<U>> some) => MapOptional(some, _ => Optional<U>.Empty());

        /// <summary>
        /// Maps the current optional instance value to the target type using the specified delegate.
        /// The delegate will be applied only if optional contains a value,
        /// otherwise the empty delegate will be applied.
        /// </summary>
        /// <typeparam name="U">The expected type of result.</typeparam>
        /// <param name="some">The delegate which would be used in case that the optional contains value.</param>
        /// <param name="empty">The delegate which would be used if the optional contains no value.</param>
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
        /// Filters the current optional instance applying the specified predicate.
        /// The predicate will be applied only if optional contains a value, otherwise the delegate replace value will be called.
        /// </summary>
        /// <param name="predicate">The predicate to be used in case that optional contains value.</param>
        /// <param name="replace">The delegate to be used if the current value doesn't match the predicate.</param>
        /// <returns>An optional value matching the predicate or the replace one.</returns>
        /// <exception cref="ArgumentException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="replace"/> is null.</exception>
        public Optional<T> Filter(Predicate<T> predicate, Func<T> replace)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (replace is null) throw new ArgumentNullException(nameof(replace));

            return this.Any() && predicate(this.Single()) ? this.Single() : replace();
        }

        /// <summary>
        /// Filters the current optional instance applying the specified predicate.
        /// The predicate will be applied only if optional contains a value, otherwise the delegate replace value will be called.
        /// </summary>
        /// <param name="predicate">The predicate to be used in case that optional contains value.</param>
        /// <param name="replaceProducer">The delegate to be used if the current value doesn't match the predicate.</param>
        /// <returns>An optional value matching the predicate or the replace one.</returns>
        /// <exception cref="ArgumentException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="replaceProducer"/> is null.</exception>
        public Optional<T> FilterOptional(Predicate<Optional<T>> predicate, Func<Optional<T>> replaceProducer)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (replaceProducer is null) throw new ArgumentNullException(nameof(replaceProducer));

            return this.Any() && predicate(this) ? this : replaceProducer();
        }

        /// <summary>
        /// Transforms the value to optional according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The mapper which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null</exception>
        public Optional<T> When(Predicate<T> predicate, Func<T, T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (this.Any() && predicate(this.Single()))
                return some(this.Single());

            return this;
        }

        /// <summary>
        /// Transforms the value to optional according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise applies the reduce delegate.
        /// </summary>
        /// <param name="some">The mapper which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The reduce which would be used in case that the predicate is false.</param>
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
        /// Calls the action according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value.
        /// </summary>
        /// <param name="some">The action which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void When(Predicate<T> predicate, Action<T> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            if (this.Any() && predicate(this.Single()))
                some(this.Single());
        }

        /// <summary>
        /// Calls the action according to the specified predicate.
        /// If the predicate is true, the delegate will be applied only if the optional contains a value,
        /// otherwise calls the empty action.
        /// </summary>
        /// <param name="some">The action which would be used in case that the predicate is true.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="empty">The other which would be used in case that the predicate is false.</param>
        /// <returns>An optional instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
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
        /// This method would have to receive a delegate that produces the replacement value,
        /// which would be used in case that the optional contains no value.
        /// </summary>
        /// <param name="empty">The delegate to produce the replacement value.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> Reduce(Func<T> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? this.Single() : empty();
        }

        /// <summary>
        /// This method would have to receive a delegate that produces the replacement optional,
        /// which would be used in case that the current optional contains no value.
        /// </summary>
        /// <param name="empty">The delegate to produce the replacement value option.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> ReduceOptional(Func<Optional<T>> empty)
        {
            if (empty is null) throw new ArgumentNullException(nameof(empty));

            return this.Any() ? this : empty();
        }

        /// <summary>
        /// Executes the specified delegate only if the current optional instance contains no value.
        /// </summary>
        /// <param name="action">The delegate to be executed.</param>
        public void Reduce(Action action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            if (!this.Any()) action();
        }
    }
}