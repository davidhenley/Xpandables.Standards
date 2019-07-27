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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the commands.
    /// The command must be decorated with the <see cref="TransactionalAttribute"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to apply transaction.</typeparam>
    public sealed class TransactionCommandDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler;

        public TransactionCommandDecorator(ICommandHandler<TCommand> decoratedHandler)
            => _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var transactionAttr = command
                  .GetType()
                  .GetCustomAttributes<TransactionalAttribute>(true)
                  .SingleOrDefault()
                  ?? throw new ArgumentException(
                      $"The {typeof(TCommand).Name} is not decorated with {nameof(TransactionalAttribute)}.");

            using var scope = transactionAttr.TransactionScope;
            await _decoratedHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            scope.Complete();
        }
    }
}