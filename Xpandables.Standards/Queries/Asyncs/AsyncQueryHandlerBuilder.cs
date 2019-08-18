/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Query
{
    /// <summary>
    /// A helper class used to implement the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> interface.
    /// </summary>
    /// <typeparam name="TQuery">Type of argument to act on.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class AsyncQueryHandlerBuilder<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly Func<IQuery<TResult>, CancellationToken, Task<TResult>> _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncQueryHandlerBuilder{TQuery, TResult}"/> with the delegate to be used
        /// as <see cref="IAsyncQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IAsyncQueryHandler{TQuery, TResult}.HandleAsync(TQuery, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public AsyncQueryHandlerBuilder(Func<IQuery<TResult>, CancellationToken, Task<TResult>> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
            => await _handler(query, cancellationToken).ConfigureAwait(false);
    }
}