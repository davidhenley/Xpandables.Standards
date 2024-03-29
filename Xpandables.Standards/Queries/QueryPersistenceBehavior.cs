﻿/************************************************************************************************************
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

using System.Design.Database;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add persistence support to query.
    /// <para>This decorator uses the <see cref="IDataContext.PersistAsync(CancellationToken)"/>
    /// after a query execution.</para>
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public sealed class QueryPersistenceBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IPersistenceBehavior
    {
        private readonly IDataContext _dataContext;
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        public QueryPersistenceBehavior(IDataContext dataContext, IQueryHandler<TQuery, TResult> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(
                nameof(dataContext),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(QueryPersistenceBehavior<TQuery, TResult>),
                    nameof(dataContext)));

            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(QueryPersistenceBehavior<TQuery, TResult>),
                    nameof(decoratee)));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
