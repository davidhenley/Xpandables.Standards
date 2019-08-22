﻿/************************************************************************************************************
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

using System.ComponentModel.DataAnnotations;
using System.Design.Command;
using System.Design.Query;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Mediator
{
    /// <summary>
    /// The default implementation for <see cref="IAsyncCommandQueryHandler"/>.
    /// Implements methods to execute the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> and
    /// <see cref="IAsyncCommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class AsyncCommandQueryHandler : IAsyncCommandQueryHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public AsyncCommandQueryHandler(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public async Task HandleCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            try
            {
                if (command is null) throw new ArgumentNullException(nameof(command));

                await _serviceProvider.GetService<IAsyncCommandHandler<TCommand>>()
                   .Reduce(() => throw new NotImplementedException(
                       ErrorMessageResources.CommandQueryHandlerMissingImplementation
                        .StringFormat(nameof(TCommand))))
                   .MapAsync((handler, cancel) => handler.HandleAsync(command, cancel), cancellationToken)
                   .ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleCommandAsync)),
                    exception);
            }
        }

        public async Task<TResult> HandleQueryResultAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                return await _serviceProvider.GetService<IAsyncQueryHandler<TQuery, TResult>>()
                    .Reduce(() => throw new NotImplementedException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(nameof(TQuery))))
                    .MapAsync((handler, cancel) => handler.HandleAsync(query, cancel), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleQueryResultAsync)),
                    exception);
            }
        }

        public async Task<TResult> HandleResultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                var wrapperType = typeof(AsyncQueryHandlerWrapper<,>)
                    .MakeGenericType(new Type[] { query.GetType(), typeof(TResult) });

                return await _serviceProvider.GetService<IAsyncQueryHandlerWrapper<TResult>>(wrapperType)
                    .Reduce(() => throw new NotImplementedException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(query.GetType().Name)))
                    .MapAsync((handler, cancel) => handler.HandleAsync(query, cancel), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleResultAsync)),
                    exception);
            }
        }
    }
}