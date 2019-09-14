
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

using System.Design.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace System.Design.Query
{
    /// <summary>
    /// This class allows the application author to add logging support to query.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query that will be used as argument.</typeparam>
    /// <typeparam name="TResult">Type of the result of the query.</typeparam>
    public sealed class QueryHandlerLoggingDecorator<TQuery, TResult> :
        ObjectDescriptor<QueryHandlerLoggingDecorator<TQuery, TResult>>, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>, ILoggingDecorator
    {
        private readonly IQueryHandler<TQuery, TResult> _decoratee;
        private readonly ILoggerWrapper _loggerWrapper;

        public QueryHandlerLoggingDecorator(
            IQueryHandler<TQuery, TResult> decoratee,
            ILoggerWrapper loggerWrapper)
            : base(decoratee)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _loggerWrapper = loggerWrapper ?? throw new ArgumentNullException(nameof(loggerWrapper));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var ex = Optional<Exception>.Empty();
            _loggerWrapper.OnEntry<TQuery>(this, query);
            try
            {
                var result = await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
                _loggerWrapper.OnSuccess<TQuery, TResult>(this, query, result);
                return result;
            }
            catch (Exception exception)
            {
                ex = exception;
                _loggerWrapper.OnException(this, exception);
                throw;
            }
            finally
            {
                _loggerWrapper.OnExit<TQuery, TResult>(this, query, Optional<TResult>.Empty(), ex);
            }
        }
    }
}
