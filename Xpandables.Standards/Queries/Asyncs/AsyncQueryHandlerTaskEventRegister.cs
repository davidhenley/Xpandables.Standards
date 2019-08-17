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

using System.Design.TaskEvent;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query.
    /// <para>This decorator will call the <see cref="AsyncTaskEventRegister"/> before and after the query execution.</para>
    /// </summary>
    /// <typeparam name="TCriteria">Type of the query.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class AsyncQueryHandlerTaskEventRegister<TCriteria, TResult> : IAsyncQueryHandler<TCriteria, TResult>
        where TCriteria : class, IQuery<TResult>
    {
        private readonly IAsyncQueryHandler<TCriteria, TResult> _decoratee;
        private readonly AsyncTaskEventRegister _eventRegister;

        public AsyncQueryHandlerTaskEventRegister(
            AsyncTaskEventRegister eventRegister,
            IAsyncQueryHandler<TCriteria, TResult> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public async Task<TResult> HandleAsync(TCriteria criteria, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _decoratee.HandleAsync(criteria, cancellationToken).ConfigureAwait(false);
                await _eventRegister.OnPostEventAsync().ConfigureAwait(false);
                return result;
            }
            catch
            {
                await _eventRegister.OnRollbackEventAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}