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
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add persistence support to command.
    /// <para>This decorator uses the <see cref="IDataContext.PersistAsync(CancellationToken)"/>
    /// after a command execution.</para>
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandHandlerPersistenceDecorator<TCommand> :
        ObjectDescriptor<CommandHandlerPersistenceDecorator<TCommand>>, ICommandHandler<TCommand>
        where TCommand : class, ICommand, IPersistenceDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandHandlerPersistenceDecorator(
            IDataContext dataContext,
            ICommandHandler<TCommand> decoratee)
            : base(decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(
                nameof(dataContext),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerPersistenceDecorator<TCommand>),
                    nameof(DataContextProviderSeederDecorator)));

            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerPersistenceDecorator<TCommand>),
                    nameof(decoratee)));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}