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

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add visitor support to a command.
    /// <para>This decorator uses the <see cref="CompositeVisitor{TArgument}"/>.</para>
    /// <para>The command must implement the <see cref="IVisitable"/> interface.</para>
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class AsyncVisitorCommandDecorator<TCommand> : IAsyncCommandHandler<TCommand>
         where TCommand : class, ICommand, IVisitable
    {
        private readonly ICompositeVisitor<TCommand> _visitor;
        private readonly IAsyncCommandHandler<TCommand> _decoratee;

        public AsyncVisitorCommandDecorator(IAsyncCommandHandler<TCommand> decoratee, ICompositeVisitor<TCommand> visitor)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            command.Accept(_visitor);
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}