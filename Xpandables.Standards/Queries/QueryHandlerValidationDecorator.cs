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

using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add validation support before a query is handled.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryHandlerValidationDecorator<TQuery, TResult> :
        ObjectDescriptor<QueryHandlerValidationDecorator<TQuery, TResult>>, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IValidationDecorator
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICustomCompositeValidator<TQuery> _validator;

        public QueryHandlerValidationDecorator(
            IQueryHandler<TQuery, TResult> decoratee,
            ICustomCompositeValidator<TQuery> validator)
            : base(decoratee)
        {
            _decoratee = decoratee;
            _validator = validator;
        }

        public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            _validator.Validate(query);
            return _decoratee.HandleAsync(query, cancellationToken);
        }
    }
}