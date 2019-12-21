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

using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the control command flow.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to apply transaction.</typeparam>
    public sealed class CommandTransactionBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, ITransactionBehavior
    {
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandTransactionBehavior(ICommandHandler<TCommand> decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandTransactionBehavior<TCommand>),
                    nameof(decoratee)));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var attribute = typeof(TCommand).GetAttribute<SupportTransactionAttribute>();

            if (attribute.IsValue())
            {
                using TransactionScope scope = attribute
                    .Map(attr => attr.GetTransactionScope()).GetValueOrDefault();
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