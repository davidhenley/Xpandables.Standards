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

using System.Design.Database;

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add persistence support to command.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandHandlerPersistence<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandHandlerPersistence(IDataContext dataContext, ICommandHandler<TCommand> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public void Handle(TCommand command)
        {
            _decoratee.Handle(command);
            _dataContext.Persist();
        }
    }
}