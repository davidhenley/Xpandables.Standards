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
    /// Defines a set of methods to automatically handle <see cref="ICommand"/> and <see cref="IQuery{TResult}"/>
    /// when targeting <see cref="IQueryHandler{TQuery, TResult}"/> or/and <see cref="ICommandHandler{TCommand}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Handles the specified query and returns the expected result.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        TResult HandleQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>;

        /// <summary>
        /// Handles the specified query and returns the expected result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        TResult HandleResult<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="NotImplementedException">The corresponding handler is missing.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void HandleCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand;
    }
}