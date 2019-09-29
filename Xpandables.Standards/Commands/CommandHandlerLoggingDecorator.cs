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

using System.Design.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Command
{
    /// <summary>
    /// This class allows the application author to add logging support to command.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public sealed class CommandHandlerLoggingDecorator<TCommand> :
        ObjectDescriptor<CommandHandlerLoggingDecorator<TCommand>>, ICommandHandler<TCommand>
        where TCommand : class, ICommand, ILoggingDecorator
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ILoggerWrapper _loggerWrapper;

        public CommandHandlerLoggingDecorator(
            ICommandHandler<TCommand> decoratee,
            ILoggerWrapper loggerWrapper)
            : base(decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerLoggingDecorator<TCommand>),
                    nameof(decoratee)));

            _loggerWrapper = loggerWrapper ?? throw new ArgumentNullException(
                nameof(loggerWrapper),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandHandlerLoggingDecorator<TCommand>),
                    nameof(loggerWrapper)));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var ex = Optional<Exception>.Empty();
            _loggerWrapper.OnEntry<TCommand>(this, command);
            try
            {
                await _decoratee.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                _loggerWrapper.OnSuccess<TCommand, object>(this, command, Optional<object>.Empty());
            }
            catch (Exception exception)
            {
                ex = exception;
                _loggerWrapper.OnException(this, exception);
                throw;
            }
            finally
            {
                _loggerWrapper.OnExit<TCommand, object>(this, command, Optional<object>.Empty(), ex);
            }
        }
    }
}
