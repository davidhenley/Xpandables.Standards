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
using System.Design.Command;
using System.Design.Query;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add validation support before a query/command is handled.
    /// </summary>
    public sealed class CommandQueryHandlerValidator : ICommandQueryHandler
    {
        private readonly ICommandQueryHandler _decoratee;
        private readonly IServiceProvider _serviceProvider;

        public CommandQueryHandlerValidator(ICommandQueryHandler decoratee, IServiceProvider serviceProvider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TResult HandleResult<TResult>(IQuery<TResult> query)
        {
            DoValidation(query);
            return _decoratee.HandleResult(query);
        }

        public TResult HandleQueryResult<TQuery, TResult>(TQuery query)
            where TQuery : class, IQuery<TResult>
        {
            DoValidation(query);
            return _decoratee.HandleQueryResult<TQuery, TResult>(query);
        }

        public void HandleCommand<TCommand>(TCommand command)
            where TCommand : class, ICommand
        {
            DoValidation(command);
            _decoratee.HandleCommand(command);
        }

        private void DoValidation<T>(T argument)
            where T : class
        {
            var validator = _serviceProvider.GetService<ICustomCompositeValidator<T>>();
            validator.Map(val => val.Validate(argument));
        }
    }
}
