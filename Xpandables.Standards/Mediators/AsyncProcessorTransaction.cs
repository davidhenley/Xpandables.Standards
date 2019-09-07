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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add transaction support to command/query handler.
    /// </summary>
    public sealed class AsyncProcessorTransaction : IAsyncProcessor
    {
        private readonly IAsyncProcessor _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public AsyncProcessorTransaction(IAsyncProcessor decoratee, IAttributeAccessor attributeAccessor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(nameof(attributeAccessor));
        }

        public async Task<TResult> HandleResultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(query.GetType());
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                var result = await _decoratee.HandleResultAsync(query, cancellationToken).ConfigureAwait(false);
                scope.Complete();

                return result;
            }

            return await _decoratee.HandleResultAsync(query, cancellationToken).ConfigureAwait(false);
        }

        public async Task HandleCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TCommand));
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                await _decoratee.HandleCommandAsync(command, cancellationToken).ConfigureAwait(false);

                scope.Complete();
            }
            else
            {
                await _decoratee.HandleCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<TResult> HandleQueryResultAsync<TQuery, TResult>(
            TQuery query, CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TQuery));
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                var result = await _decoratee
                    .HandleQueryResultAsync<TQuery, TResult>(query, cancellationToken).ConfigureAwait(false);
                scope.Complete();

                return result;
            }

            return await _decoratee.HandleQueryResultAsync<TQuery, TResult>(query, cancellationToken).ConfigureAwait(false);
        }
    }

}
