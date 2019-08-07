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

using System.Threading;
using System.Threading.Tasks;

namespace System.Patterns
{
    /// <summary>
    /// The default implementation for <see cref="IAsyncMediator"/>.
    /// Implements methods to execute the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> and
    /// <see cref="IAsyncCommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class AsyncMediator : IAsyncMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public AsyncMediator(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public async Task HandleCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            try
            {
                var handler = _serviceProvider.GetService<IAsyncCommandHandler<TCommand>>();
                await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    $"{nameof(HandleCommandAsync)} operation failed. See inner exception",
                    exception);
            }
        }

        public async Task<TResult> HandleQueryAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                var handler = _serviceProvider.GetService<IAsyncQueryHandler<TQuery, TResult>>();
                return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    $"{nameof(HandleQueryAsync)} operation failed. See inner exception",
                    exception);
            }
        }

        public async Task<TResult> HandleQueryResultAsync<TResult>(
           IQuery<TResult> query,
           CancellationToken cancellationToken = default)
           where TResult : class
        {
            try
            {
                var wrapperType = typeof(AsyncQueryHandlerWrapper<,>).MakeGenericType(new Type[] { query.GetType(), typeof(TResult) });
                var wrapperHandler = _serviceProvider.GetService<IAsyncQueryHandlerWrapper<TResult>>(wrapperType);
                return await wrapperHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    $"{nameof(HandleQueryAsync)} operation failed. See inner exception",
                    exception);
            }
        }
    }
}