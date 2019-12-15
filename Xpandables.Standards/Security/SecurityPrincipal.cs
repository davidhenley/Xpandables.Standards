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

using System.Design.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Security.Claims;

namespace System.Design
{
    /// <summary>
    /// Provides with a principal that holds information of user identities.
    /// This class is used with <see cref="ISecurityPrincipalBehavior"/>
    /// and its decorator class.
    /// </summary>
    public abstract class SecurityPrincipal : IFluent
    {
        /// <summary>
        /// May contains the principal if exist.
        /// This value is provided by the
        /// </summary>
        protected Optional<ClaimsPrincipal> Principal { get; private set; } = Optional<ClaimsPrincipal>.Empty();

        /// <summary>
        /// Sets the <see cref="Principal"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="principal">The principal to be used.</param>
        internal void SetPrincipal(ClaimsPrincipal principal) => Principal = principal;
    }

    /// <summary>
    /// Provides with a principal that holds information of user identities.
    /// This class is used with <see cref="ISecurityPrincipalBehavior"/>
    /// and its decorator class.
    /// This class implements the <see cref="IQueryExpression{TSource}"/> interface.
    /// You must override the <see cref="BuildExpression"/> method in order to provide a custom behavior.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public abstract class SecurityPrincipal<TSource> : SecurityPrincipal, IQueryExpression<TSource>
        where TSource : class
    {
        public Expression<Func<TSource, bool>> Expression() => BuildExpression();

        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        protected virtual Expression<Func<TSource, bool>> BuildExpression() => PredicateBuilder.New<TSource>();

        [SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Expression<Func<TSource, bool>>([NotNull] SecurityPrincipal<TSource> criteria)
             => criteria?.Expression() ?? PredicateBuilder.New<TSource>();

        [SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Func<TSource, bool>(SecurityPrincipal<TSource> criteria)
            => criteria?.Expression().Compile() ?? PredicateBuilder.New<TSource>();
    }
}
