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

using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add validation support before a command is handled.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public sealed class CommandValidationBehavior<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand, IValidationBehavior
    {
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly ICompositeValidatorRule<TCommand> _validator;

        public CommandValidationBehavior(ICommandHandler<TCommand> decoratee, ICompositeValidatorRule<TCommand> validator)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandValidationBehavior<TCommand>),
                    nameof(decoratee)));

            _validator = validator ?? throw new ArgumentNullException(
                nameof(validator),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(CommandValidationBehavior<TCommand>),
                    nameof(validator)));
        }

        public Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _validator.Validate(command);
            return _decoratee.HandleAsync(command, cancellationToken);
        }
    }
}