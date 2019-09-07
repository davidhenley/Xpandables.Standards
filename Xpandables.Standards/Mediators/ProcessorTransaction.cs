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
using System.Transactions;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add transaction support to command/query handler.
    /// </summary>
    public sealed class ProcessorTransaction : IProcessor
    {
        private readonly IProcessor _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public ProcessorTransaction(IProcessor decoratee, IAttributeAccessor attributeAccessor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(nameof(attributeAccessor));
        }

        public TResult HandleResult<TResult>(IQuery<TResult> query)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(query.GetType());
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                var result = _decoratee.HandleResult(query);
                scope.Complete();

                return result;
            }

            return _decoratee.HandleResult(query);
        }

        public void HandleCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TCommand));
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                _decoratee.HandleCommand(command);

                scope.Complete();
            }
            else
            {
                _decoratee.HandleCommand(command);
            }
        }

        public TResult HandleQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var transactionAttribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TQuery));
            if (transactionAttribute.Any())
            {
                using var scope = transactionAttribute.Single().GetTransactionScope();
                var result = _decoratee.HandleQueryResult<TQuery, TResult>(query);
                scope.Complete();

                return result;
            }

            return _decoratee.HandleQueryResult<TQuery, TResult>(query);
        }
    }
}
