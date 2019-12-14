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
    /// This class allows the application author to add user support to command.
    /// <para>This decorator uses the <see cref="ISecurityUserBehavior"/>
    /// before a command execution.</para>
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandSecurityUserBehavior<TUser, TCommand> : ICommandHandler<TCommand>
        where TUser : class
        where TCommand : SecurityUser<TUser>, ICommand, ISecurityUserBehavior
    {
        private readonly ISecurityUserProvider<TUser> _securityUserProvider;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommandSecurityUserBehavior(
            ISecurityUserProvider<TUser> securityUserProvider,
            ICommandHandler<TCommand> decoratee)
        {
            _securityUserProvider = securityUserProvider
                ?? throw new ArgumentNullException(nameof(securityUserProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var user = _securityUserProvider.GetUser();
            command.SetUser(user);
            await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
