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

using System.Transactions;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the queries.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class QueryHandlerTransaction<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public QueryHandlerTransaction(IQueryHandler<TQuery, TResult> decoratee, IAttributeAccessor attributeAccessor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(nameof(attributeAccessor));
        }

        public TResult Handle(TQuery query)
        {
            using var scope = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TQuery))
                           .Reduce(() => throw new ArgumentException(
                                    $"{typeof(TQuery).Name} is not decorated with {nameof(SupportTransactionAttribute)}"))
                           .Cast<SupportTransactionAttribute>()
                           .GetTransactionScope();

            var result = _decoratee.Handle(query);
            scope.Complete();

            return result;
        }
    }
}
