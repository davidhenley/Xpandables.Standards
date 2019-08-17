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

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add validation support before a query is handled.
    /// </summary>
    /// <typeparam name="TCriteria">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class QueryHandlerValidator<TCriteria, TResult> : IQueryHandler<TCriteria, TResult>
        where TCriteria : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TCriteria, TResult> _decoratee;
        private readonly ICustomCompositeValidator<TCriteria> _validator;

        public QueryHandlerValidator(
            IQueryHandler<TCriteria, TResult> decoratee,
            ICustomCompositeValidator<TCriteria> validator)
        {
            _decoratee = decoratee;
            _validator = validator;
        }

        public TResult Handle(TCriteria criteria)
        {
            _validator.Validate(criteria);
            return _decoratee.Handle(criteria);
        }
    }
}