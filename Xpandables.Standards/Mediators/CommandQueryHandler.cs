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
using System.Design.Command;
using System.Design.Query;

namespace System.Design.Mediator
{
    /// <summary>
    /// The default implementation for <see cref="ICommandQueryHandler"/>.
    /// Implements methods to execute the <see cref="IQueryHandler{TQuery, TResult}"/> and
    /// <see cref="ICommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class CommandQueryHandler : ICommandQueryHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandQueryHandler(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public void HandleCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand
        {
            try
            {
                if (command is null) throw new ArgumentNullException(nameof(command));

                _serviceProvider.GetService<ICommandHandler<TCommand>>()
                   .Reduce(() => throw new NotImplementedException(
                       ErrorMessageResources.CommandQueryHandlerMissingImplementation
                        .StringFormat(nameof(TCommand))))
                   .Map(handler => handler.Handle(command));
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleCommand)),
                    exception);
            }
        }

        public TResult HandleQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                return _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()
                    .Reduce(() => throw new NotImplementedException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(nameof(TQuery))))
                    .Map(handler => handler.Handle(query));
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleCommand)),
                    exception);
            }
        }

        public TResult HandleResult<TResult>(IQuery<TResult> query)
        {
            try
            {
                if (query is null) throw new ArgumentNullException(nameof(query));

                var wrapperType = typeof(QueryHandlerWrapper<,>)
                    .MakeGenericType(new Type[] { query.GetType(), typeof(TResult) });

                return _serviceProvider.GetService<IQueryHandlerWrapper<TResult>>(wrapperType)
                    .Reduce(() => throw new NotImplementedException(
                        ErrorMessageResources.CommandQueryHandlerMissingImplementation
                            .StringFormat(query.GetType().Name)))
                    .Map(handler => handler.Handle(query));
            }
            catch (Exception exception) when (!(exception is ArgumentException)
                                            && !(exception is ValidationException)
                                            && !(exception is NotImplementedException)
                                            && !(exception is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleCommand)),
                    exception);
            }
        }
    }
}