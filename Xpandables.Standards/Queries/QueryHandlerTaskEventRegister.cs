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

using System.Design.TaskEvent;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to query.
    /// </summary>
    /// <typeparam name="TCriteria">Type of the query to apply transaction.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryHandlerTaskEventRegister<TCriteria, TResult> : IQueryHandler<TCriteria, TResult>
        where TCriteria : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TCriteria, TResult> _decoratee;
        private readonly TaskEventRegister _eventRegister;

        public QueryHandlerTaskEventRegister(
            TaskEventRegister eventRegister,
            IQueryHandler<TCriteria, TResult> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public TResult Handle(TCriteria criteria)
        {
            try
            {
                var result = _decoratee.Handle(criteria);
                _eventRegister.OnPostEvent();
                return result;
            }
            catch
            {
                _eventRegister.OnRollbackEvent();
                throw;
            }
        }
    }
}