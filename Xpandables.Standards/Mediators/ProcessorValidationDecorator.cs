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

using System.ComponentModel.DataAnnotations;
using System.Design.Command;
using System.Design.DependencyInjection;
using System.Design.Query;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Mediator
{
    /// <summary>
    /// This class allows the application author to add validation support before a query/command is handled.
    /// </summary>
    public sealed class ProcessorValidationDecorator : IProcessor
    {
        private readonly IProcessor _decoratee;
        private readonly IServiceProvider _serviceProvider;

        public ProcessorValidationDecorator(IProcessor decoratee, IServiceProvider serviceProvider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(
                nameof(decoratee),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(ProcessorValidationDecorator),
                    nameof(decoratee)));

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(
                nameof(serviceProvider),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(ProcessorValidationDecorator),
                    nameof(serviceProvider)));
        }

        public ValueTask<TResult> HandleResultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            DoValidation(query);
            return _decoratee.HandleResultAsync(query, cancellationToken)
;
        }

        public ValueTask<TResult> HandleQueryResultAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>
        {
            DoValidation(query);
            return _decoratee.HandleQueryResultAsync<TQuery, TResult>(query, cancellationToken)
;
        }

        public Task HandleCommandAsync<TCommand>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : class, ICommand
        {
            DoValidation(command);
            return _decoratee.HandleCommandAsync(command, cancellationToken);
        }

        private void DoValidation<T>(T argument)
            where T : class
        {
            var validator = _serviceProvider.GetServiceExtended<ICompositeValidatorRule<T>>();
            validator.Map(val => val.Validate(argument));
        }
    }
}
