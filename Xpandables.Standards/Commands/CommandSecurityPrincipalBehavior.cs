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

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add principal support to command.
    /// <para>This decorator uses the <see cref="ISecurityPrincipalProvider"/>
    /// before a command execution.</para>
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandSecurityPrincipalBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : SecurityPrincipal, ICommand, ISecurityPrincipalBehavior
    {
        private readonly ISecurityPrincipalProvider _securityPrincipalProvider;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandSecurityPrincipalBehavior(
            ISecurityPrincipalProvider securityPrincipalProvider,
            ICommandHandler<TCommand> decoratee)
        {
            _securityPrincipalProvider = securityPrincipalProvider
                ?? throw new ArgumentNullException(nameof(securityPrincipalProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var principal = _securityPrincipalProvider.GetPrincipal();
            command.SetPrincipal(principal);
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
