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

using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace System.Patterns
{
    /// <summary>
    /// This class allows the application author to add persistence support to all of the query handlers.
    /// <para>This decorator uses the <see cref="IDataContext.PersistEntitiesAsync(CancellationToken)"/> after a query execution.</para>
    /// </summary>
    /// <typeparam name="TQuery">Type of the query to apply transaction.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public sealed class PersistenceQueryDecorator<TQuery, TResult> : Disposable, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IDataContext _dataContext;
        private readonly IQueryHandler<TQuery, TResult> _decoratedHandler;

        public PersistenceQueryDecorator(IDataContext dataContext, IQueryHandler<TQuery, TResult> decoratedHandler)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _decoratedHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            await _dataContext.PersistEntitiesAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}