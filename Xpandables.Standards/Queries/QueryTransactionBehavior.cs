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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the queries.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class QueryTransactionBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ITransactionBehavior
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        public QueryTransactionBehavior(IQueryHandler<TQuery, TResult> decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(QueryTransactionBehavior<TQuery, TResult>),
                    nameof(decoratee)));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var attribute = typeof(TQuery).GetAttribute<SupportTransactionAttribute>();

            if (attribute.IsValue())
            {
                using TransactionScope scope = attribute
                    .Map(attr => attr.GetTransactionScope()).GetValueOrDefault();
                var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                scope.Complete();
                return result;
            }
            else
            {
                return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
