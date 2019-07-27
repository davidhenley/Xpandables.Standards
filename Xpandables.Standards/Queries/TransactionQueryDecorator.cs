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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the queries.
    /// The query must be decorated with the <see cref="TransactionalAttribute"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query to apply transaction.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class TransactionQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratedHandler;

        public TransactionQueryDecorator(IQueryHandler<TQuery, TResult> decoratedHandler)
            => _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var transactionAttr = query
                  .GetType()
                  .GetCustomAttributes<TransactionalAttribute>(true)
                  .SingleOrDefault()
                  ?? throw new ArgumentException(
                      $"The {typeof(TQuery).Name} is not decorated with {nameof(TransactionalAttribute)}.");

            using var scope = transactionAttr.TransactionScope;
            var result = await _decoratedHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            scope.Complete();

            return result;
        }
    }
}