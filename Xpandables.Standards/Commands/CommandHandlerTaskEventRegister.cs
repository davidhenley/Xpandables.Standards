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

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add post/rollback event support to command.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandHandlerTaskEventRegister<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly TaskEventRegister _eventRegister;

        public CommandHandlerTaskEventRegister(
            TaskEventRegister eventRegister,
            ICommandHandler<TCommand> decoratee)
        {
            _eventRegister = eventRegister ?? throw new ArgumentNullException(nameof(eventRegister));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public void Handle(TCommand command)
        {
            try
            {
                _decoratee.Handle(command);
                _eventRegister.OnPostEvent();
            }
            catch
            {
                _eventRegister.OnRollbackEvent();
                throw;
            }
        }
    }
}