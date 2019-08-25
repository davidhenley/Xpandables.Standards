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

namespace System.Design.Query
{
    /// <summary>
    /// Implementation for <see cref="IQueryHandlerWrapper{TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <remarks>
    /// From https://gist.github.com/dotnetjunkie/d9bdb09534a75635ca552755faaa1cd5
    /// </remarks>
    public sealed class QueryHandlerWrapper<TQuery, TResult> : IQueryHandlerWrapper<TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        public QueryHandlerWrapper(IQueryHandler<TQuery, TResult> decoratee)
            => _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));

        public TResult Handle(IQuery<TResult> query) => _decoratee.Handle((TQuery)query);
    }
}
