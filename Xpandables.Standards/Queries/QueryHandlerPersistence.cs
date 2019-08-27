
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

using System.Design.Database;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add persistence support to query.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">the type of the result.</typeparam>
    public sealed class QueryHandlerPersistence<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IDataContext _dataContext;
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        public QueryHandlerPersistence(IDataContext dataContext, IQueryHandler<TQuery, TResult> decoratee)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public TResult Handle(TQuery query)
        {
            var result = _decoratee.Handle(query);
            _dataContext.Persist();
            return result;
        }
    }
}
