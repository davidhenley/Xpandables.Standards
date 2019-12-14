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

using System.Threading;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// This class allows the application author to add user support to query.
    /// <para>This decorator uses the <see cref="ISecurityUserBehavior"/>
    /// before a query execution.</para>
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public sealed class QuerySecurityUserBehavior<TUser, TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TUser : class
        where TQuery : SecurityUser<TUser>, IQuery<TResult>, ISecurityPrincipalBehavior
    {
        private readonly ISecurityUserProvider<TUser> _securutyUserProvider;
        private readonly IQueryHandler<TQuery, TResult> _decoratee;

        public QuerySecurityUserBehavior(
            ISecurityUserProvider<TUser> securityUserProvider,
            IQueryHandler<TQuery, TResult> decoratee)
        {
            _securutyUserProvider = securityUserProvider
                ?? throw new ArgumentNullException(nameof(securityUserProvider));
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var user = _securutyUserProvider.GetUser();
            query.SetUser(user);
            return await _decoratee.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}
