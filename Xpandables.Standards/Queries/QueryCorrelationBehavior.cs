﻿/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query.
    /// <para>This decorator will call the <see cref="CorrelationTask"/> before and after the query execution.</para>
    /// </summary>
    /// <typeparam name="TQuery">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryCorrelationBehavior<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ICorrelationBehavior
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly CorrelationTask _eventRegister;

        public QueryCorrelationBehavior(CorrelationTask eventRegister, IQueryHandler<TQuery, TResult> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(
                nameof(eventRegister),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(QueryCorrelationBehavior<TQuery, TResult>),
                    nameof(eventRegister)));

            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(QueryCorrelationBehavior<TQuery, TResult>),
                    nameof(decoratee)));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                await _eventRegister.OnPostEventAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                await _eventRegister.OnRollbackEventAsync(exception).ConfigureAwait(false);
                throw;
            }
        }
    }
}