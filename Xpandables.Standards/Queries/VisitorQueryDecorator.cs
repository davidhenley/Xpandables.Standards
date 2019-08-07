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

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add visitor support to a query.
    /// <para>This decorator uses the <see cref="CompositeVisitor{TArgument}"/>.</para>
    /// <para>The query must implement the <see cref="IVisitable"/> interface.</para>
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public sealed class VisitorQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
         where TQuery : class, IQuery<TResult>, IVisitable
    {
        private readonly ICompositeVisitor<TQuery> _visitor;
        private readonly IQueryHandler<TQuery, TResult> decoratee;

        public VisitorQueryDecorator(IQueryHandler<TQuery, TResult> decoratee, ICompositeVisitor<TQuery> visitor)
        {
            this.decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        public TResult Handle(TQuery query)
        {
            query.Accept(_visitor);
            return decoratee.Handle(query);
        }
    }
}