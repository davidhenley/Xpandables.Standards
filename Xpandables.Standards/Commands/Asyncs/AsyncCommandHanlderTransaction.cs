﻿/************************************************************************************************************
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

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add transaction support to all of the commands.
    /// The command must be decorated with the <see cref="SupportTransactionAttribute"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to apply transaction.</typeparam>
    public sealed class AsyncCommandHanlderTransaction<TCommand> : IAsyncCommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly IAsyncCommandHandler<TCommand> _decoratee;
        private readonly IAttributeAccessor _attributeAccessor;

        public AsyncCommandHanlderTransaction(IAsyncCommandHandler<TCommand> decoratee, IAttributeAccessor attributeAccessor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _attributeAccessor = attributeAccessor ?? throw new ArgumentNullException(nameof(attributeAccessor));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            using var scope = _attributeAccessor.GetAttribute<SupportTransactionAttribute>(typeof(TCommand))
                .Reduce(() => throw new ArgumentException(
                         $"{typeof(TCommand).Name} is not decorated with {nameof(SupportTransactionAttribute)}"))
                .Cast<SupportTransactionAttribute>()
                .GetTransactionScope();

            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            scope.Complete();
        }
    }
}