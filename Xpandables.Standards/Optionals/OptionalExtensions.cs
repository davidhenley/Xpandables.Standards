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
    /// <summary>
    /// Functionality for optional pattern methods.
    /// </summary>
    public static partial class OptionalExtensions
    {
        /// <summary>
        /// Converts the left and right values to optional pair.
        /// If one of the value is null, returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <typeparam name="TU">The type of the right value.</typeparam>
        /// <param name="left">The left value to act on.</param>
        /// <param name="right">The right value to act on.</param>
        public static Optional<(T Left, TU Right)> And<T, TU>(this T left, TU right) => left.AsOptional().And(right);

        /// <summary>
        /// Returns an optional that contains the value if that value matches the predicate.
        /// Otherwise returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value to act on.</param>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>An optional of <typeparamref name="T"/> value.</returns>
        public static Optional<T> When<T>(this T source, bool predicate) => predicate ? source.AsOptional() : OptionalBuilder.Empty<T>();

        /// <summary>
        /// Returns an optional that contains the value if that value matches the predicate.
        /// Otherwise returns an empty optional.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value to act on.</param>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>An optional of <typeparamref name="T"/> value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Optional<T> When<T>(this T source, Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return source.When(predicate(source));
        }

        /// <summary>
        /// When source is not null, applies the <paramref name="trueAction"/> to the element if the value matches the predicate,
        /// otherwise applies the <paramref name="falseAction"/>.
        /// If source is null, returns an empty optional of <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <typeparam name="TU">The type of the result.</typeparam>
        /// <param name="source">The value to act on.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="trueAction">The delegate to be executed on true predicate with value.</param>
        /// <param name="falseAction">The delegate to be executed</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="trueAction"/> is null</exception>
        /// /// <exception cref="ArgumentNullException">The <paramref name="falseAction"/> is null</exception>
        public static Optional<TU> When<T, TU>(this T source, Predicate<T> predicate, Func<T, TU> trueAction, Func<T, TU> falseAction)
        {
            if (trueAction is null) throw new ArgumentNullException(nameof(trueAction));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (falseAction is null) throw new ArgumentNullException(nameof(falseAction));

            return source.AsOptional().When(predicate, trueAction, falseAction);
        }
    }
}