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
                var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
                handler.Handle(command);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.CommandQueryHandlerFailed.StringFormat(nameof(HandleCommand)),
                    exception);
            }
        }

        public TResult HandleQuery<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>();
                return handler.Handle(query);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    $"{nameof(HandleQuery)} operation failed. See inner exception",
                    exception);
            }
        }

        public TResult HandleQueryResult<TResult>(IQuery<TResult> query)
           where TResult : class
        {
            try
            {
                var wrapperType = typeof(QueryHandlerWrapper<,>).MakeGenericType(new Type[] { query.GetType(), typeof(TResult) });
                var wrapperHandler = _serviceProvider.GetService<IQueryHandlerWrapper<TResult>>(wrapperType);
                return wrapperHandler.Handle(query);
            }
            catch (Exception exception) when (!(exception is ArgumentNullException)
                                            && !(exception is InvalidOperationException)
                                            && !(exception is OperationCanceledException))
            {
                throw new InvalidOperationException(
                    $"{nameof(HandleQuery)} operation failed. See inner exception",
                    exception);
            }
        }
    }
}