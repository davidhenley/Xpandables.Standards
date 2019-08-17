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
    /// <typeparam name="TQuery">Type of the query to apply transaction.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class QueryHandlerTaskEventRegister<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly TaskEventRegister _eventRegister;

        public QueryHandlerTaskEventRegister(
            TaskEventRegister eventRegister,
            IQueryHandler<TQuery, TResult> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public TResult Handle(TQuery query)
        {
            try
            {
                var result = _decoratee.Handle(query);
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