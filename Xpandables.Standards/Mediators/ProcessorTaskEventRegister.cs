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

using System.Design.Command;
using System.Design.Query;
using System.Design.TaskEvent;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to command/query handler.
    /// </summary>
    public sealed class ProcessorTaskEventRegister : IProcessor
    {
        private readonly IProcessor _decoratee;
        private readonly TaskEventRegister _eventRegister;

        public ProcessorTaskEventRegister(IProcessor decoratee, TaskEventRegister eventRegister)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
        }

        public TResult HandleResult<TResult>(IQuery<TResult> query)
        {
            try
            {
                var result = _decoratee.HandleResult(query);
                _eventRegister.OnPostEvent();

                return result;
            }
            catch
            {
                _eventRegister.OnRollbackEvent();
                throw;
            }
        }

        public void HandleCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand
        {
            try
            {
                _decoratee.HandleCommand(command);
                _eventRegister.OnPostEvent();
            }
            catch
            {
                _eventRegister.OnRollbackEvent();
                throw;
            }
        }

        public TResult HandleQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>
        {
            try
            {
                var result = _decoratee.HandleQueryResult<TQuery, TResult>(query);
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
