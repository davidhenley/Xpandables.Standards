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

using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add persistence support to command.
    /// <para>This decorator uses the <see cref="IDataContext.PersistEntitiesAsync(CancellationToken)"/> after a command execution.</para>
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class PersistenceCommandDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly IDataContext _dataContext;
        private readonly ICommandHandler<TCommand> _decoratedHandler;

        public PersistenceCommandDecorator(IDataContext dataContext, ICommandHandler<TCommand> decoratedHandler)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _decoratedHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistEntitiesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}