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

using System.Threading;
using System.Threading.Tasks;

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
        private readonly IQueryHandler<TQuery, TResult> _decoratedHandler;

        public VisitorQueryDecorator(IQueryHandler<TQuery, TResult> decoratedHandler, ICompositeVisitor<TQuery> visitor)
        {
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            query.Accept(_visitor);

            return await _decoratedHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}