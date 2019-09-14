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

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the queries.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class QueryHandlerTransactionDecorator<TQuery, TResult> :
        ObjectDescriptor<QueryHandlerTransactionDecorator<TQuery, TResult>>, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ITransactionDecorator
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public QueryHandlerTransactionDecorator(
            IQueryHandler<TQuery, TResult> decoratee,
            IAttributeAccessor attributeAccessor)
            : base(decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(nameof(attributeAccessor));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var attribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TQuery));

            if (attribute.IsValue())
            {
                using (var scope = attribute.Single().GetTransactionScope())
                {
                    var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                    scope.Complete();

                    return result;
                }
            }
            else
            {
                return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
