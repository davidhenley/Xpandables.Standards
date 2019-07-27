/************************************************************************************************************
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

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query.
    /// <para>This decorator will call the <see cref="EventRegister"/> before and after the query execution.</para>
    /// </summary>
    /// <typeparam name="TQuery">Type of the query to apply transaction.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class EventRegisterQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICorrelationContext _correlationContext;
        private readonly EventRegister _eventRegister;

        public EventRegisterQueryDecorator(
            EventRegister eventRegister, IQueryHandler<TQuery, TResult> decoratee, ICorrelationContext correlationContext)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
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
                _correlationContext.SetOrUpdateValue("Exception", exception);
                await _eventRegister.OnRollbackEventAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}