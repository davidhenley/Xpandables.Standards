﻿/************************************************************************************************************
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

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add validation support before a query is handled.
    /// <para>This decorator uses the <see cref="CompositeValidator{TArgument}"/>.</para>
    /// <para>The query must implement the <see cref="IValidatableAttribute"/> interface.</para>
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class ValidatorQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, IValidatableAttribute
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ICompositeValidator<TQuery> _validator;

        public ValidatorQueryDecorator(IQueryHandler<TQuery, TResult> decoratee, ICompositeValidator<TQuery> validator)
        {
            _decoratee = decoratee;
            _validator = validator;
        }

        public TResult Handle(TQuery query)
        {
            _validator.Validate(query);
            return _decoratee.Handle(query);
        }
    }
}