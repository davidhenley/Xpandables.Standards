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
using System.Design.Database;
using System.Design.Query;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add persistence support to command/query handler.
    /// </summary>
    public sealed class ProcessorPersistenceDecorator : IProcessor
    {
        private readonly IProcessor _decoratee;
        private readonly IDataContext _dataContext;

        public ProcessorPersistenceDecorator(IProcessor decoratee, IDataContext dataContext)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(ProcessorPersistenceDecorator),
                    nameof(decoratee)));
            _dataContext = dataContext ?? throw new ArgumentNullException(
                nameof(dataContext),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(ProcessorPersistenceDecorator),
                    nameof(dataContext)));
        }

        public async Task<TResult> HandleResultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            var result = await _decoratee.HandleResultAsync(query, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task HandleCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            await _decoratee.HandleCommandAsync(command, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(default);
        }

        public async Task<TResult> HandleQueryResultAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            var result = await _decoratee.HandleQueryResultAsync<TQuery, TResult>(query, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
