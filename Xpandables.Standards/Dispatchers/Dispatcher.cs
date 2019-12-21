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

using System.ComponentModel.DataAnnotations;
using System.Design.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// The default implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IQueryHandler{TQuery, TResult}"/> and
    /// <see cref="ICommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            try
            {
                if (command is null) throw new ArgumentNullException(nameof(command));

                await _serviceProvider
                    .XGetService<ICommandHandler<TCommand>>()
                    .WhenEmpty(() => throw new ArgumentException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(nameof(TCommand))))
                   .MapAsync(handler => handler.HandleAsync(command, cancellationToken))
                   .ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(SendCommandAsync)),
                    exception);
            }
        }

        public async Task<TResult> SendQueryResultAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));
                var execution = await _serviceProvider
                    .XGetService<IQueryHandler<TQuery, TResult>>()
                    .WhenEmpty(() => throw new ArgumentException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(nameof(TQuery))))
                    .MapAsync(handler => handler.HandleAsync(query, cancellationToken))
                    .ConfigureAwait(false);

                return execution.GetValueOrDefault();
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(SendQueryResultAsync)),
                    exception);
            }
        }

        public async Task<TResult> SendQueryAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                Type wrapperType = typeof(QueryHandlerWrapper<,>)
                    .MakeGenericTypeSafe(new Type[] { query.GetType(), typeof(TResult) })
                    .WhenException(exception =>
                    {
                        throw new InvalidOperationException(
                            "Building Query wrapper failed.",
                            exception);
                    })
                    .GetValueOrDefault();

                var execution = await _serviceProvider
                    .XGetService<IQueryHandlerWrapper<TResult>>(wrapperType)
                    .WhenEmpty(() => throw new ArgumentException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(query.GetType().Name)))
                    .MapAsync(handler => handler.HandleAsync(query, cancellationToken))
                    .ConfigureAwait(false);

                return execution.GetValueOrDefault();
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is OperationCanceledException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(SendQueryAsync)),
                    exception);
            }
        }

        public async Task SendEventAsync<T>(T source, CancellationToken cancellationToken)
                  where T : class, IEvent
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var tasks = _serviceProvider
                .XGetServices<IEventHandler<T>>()
                .Select(handler => handler.HandleAsync(source));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task SendEventAsync(IEvent source, CancellationToken cancellationToken)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var typeHandler = typeof(IEventHandler<>).MakeGenericType(new Type[] { source.GetType() });

            var tasks = _serviceProvider
                .XGetServices<IEventHandler>(typeHandler)
                .Select(handler => handler.HandleAsync(source));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}