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
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Functionality for optional pattern base.
    /// </summary>
    public static partial class Optional
    {
        /// <summary>
        /// Converts the current source to optional.
        /// The result instance will contains a value only if the source is not null.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="source">An instance of value.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<T> ToOptional<T>(this T source)
            => EqualityComparer<T>.Default.Equals(source, default) ? Optional<T>.Empty() : Optional<T>.Some(source);

        /// <summary>
        /// Converts the current source to an asynchronous optional.
        /// The result instance will contains a value only if the source is not null.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="source">An instance of value.</param>
        /// <returns>An optional instance.</returns>
        public static async Task<Optional<T>> ToOptionalAsync<T>(this T source)
            => await Task.FromResult(source).ConfigureAwait(false);
    }
}