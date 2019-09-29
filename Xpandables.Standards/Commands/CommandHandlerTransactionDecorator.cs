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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the commands.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to apply transaction.</typeparam>
    public sealed class CommandHandlerTransactionDecorator<TCommand> :
        ObjectDescriptor<CommandHandlerTransactionDecorator<TCommand>>, ICommandHandler<TCommand>
        where TCommand : class, ICommand, ITransactionDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public CommandHandlerTransactionDecorator(
            ICommandHandler<TCommand> decoratee,
            IAttributeAccessor attributeAccessor)
            : base(decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerTransactionDecorator<TCommand>),
                    nameof(decoratee)));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(
                nameof(attributeAccessor),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerTransactionDecorator<TCommand>),
                    nameof(attributeAccessor)));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var attribute = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TCommand));

            if (attribute.IsValue())
            {
                using var scope = attribute.Single().GetTransactionScope();
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                scope.Complete();
            }
            else
            {
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}