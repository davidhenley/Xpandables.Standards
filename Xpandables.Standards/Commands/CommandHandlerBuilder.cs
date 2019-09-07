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

using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Command
{
    /// <summary>
    /// A helper class used to implement the <see cref="ICommandHandler{TCommand}"/> interface.
    /// </summary>
    /// <typeparam name="TCommand">Type of argument to act on.</typeparam>
    public sealed class CommandHandlerBuilder<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly Func<TCommand, CancellationToken, Task> _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandHandlerBuilder{TArgument}"/> with the delegate to be used
        /// as <see cref="ICommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/> implementation.
        /// </summary>
        /// <param name="handler">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="ICommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is null.</exception>
        public CommandHandlerBuilder(Func<TCommand, CancellationToken, Task> handler)
            => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await _handler(command, cancellationToken).ConfigureAwait(false);
    }
}