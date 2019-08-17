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

using System;
using System.Linq;

namespace System
{
#pragma warning disable CA1710 // Les identificateurs doivent avoir un suffixe correct
    /// <summary>
    /// Represents the execution state of an operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class ExecutionResult<TResult> : Optional<TResult>
#pragma warning restore CA1710 // Les identificateurs doivent avoir un suffixe correct
    {
        public ExecutionResult(TResult result)
            : base(new[] { result })
            => Exception = OptionalHelpers.Empty<Exception>();

        public ExecutionResult(Exception exception)
            : base(Array.Empty<TResult>())
            => Exception = exception ?? throw new ArgumentNullException(nameof(exception));

        /// <summary>
        /// Contains the handled exception.
        /// </summary>
        public Optional<Exception> Exception { get; }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element on exception.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<TResult> WhenException(Func<TResult> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (Exception.Any())
                return some();

            return OptionalHelpers.Empty<TResult>();
        }

        /// <summary>
        /// Creates a new element that is the result of applying the given function to the element on exception.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <returns>An optional of <typeparamref name="TResult"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<TResult> WhenException(Func<Exception, TResult> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));
            if (Exception.Any())
                return some(Exception.Single());

            return OptionalHelpers.Empty<TResult>();
        }

        /// <summary>
        /// Executes the given function on exception.
        /// </summary>
        /// <param name="some">The function to return the element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public void WhenException(Action<Exception> some)
        {
            if (some is null) throw new ArgumentNullException(nameof(some));

            if (Exception.Any())
                some(Exception.Single());
        }
    }
}
