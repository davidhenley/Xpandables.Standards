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

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add validation support before a command is handled.
    /// <para>This decorator uses the <see cref="CompositeValidator{TArgument}"/>.</para>
    /// <para>The query must implement the <see cref="IValidatableAttribute"/> interface.</para>
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class ValidatorCommandDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler;
        private readonly ICompositeValidator<TCommand> _validator;

        public ValidatorCommandDecorator(
            ICommandHandler<TCommand> decoratedhandler, ICompositeValidator<TCommand> validator)
        {
            _decoratedHandler = decoratedhandler ?? throw new ArgumentNullException(nameof(decoratedhandler));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _validator.Validate(command);

            await _decoratedHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}