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

namespace System.Design
{
    /// <summary>
    /// Provides with a class that holds information of user of specific type.
    /// This class is used with <see cref="ISecurityUserBehavior"/>
    /// and its decorator class.
    /// </summary>
    public abstract class SecurityUser : IFluent
    {
        /// <summary>
        /// May contains the principal if exist.
        /// This value is provided by an implementation of <see cref="ISecurityUserProvider"/>.
        /// </summary>
        protected Optional<IUser> User { get; private set; } = Optional<IUser>.Empty();

        /// <summary>
        /// Sets the <see cref="User"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="user">The user to be used.</param>
        internal void SetUser(Optional<IUser> user) => User = user;
    }

    /// <summary>
    /// Provides with a principal that holds information of user of specific type.
    /// This class is used with <see cref="ISecurityUserBehavior"/>
    /// and its decorator class.
    /// This class implements the <see cref="IQueryExpression{TSource}"/> interface.
    /// You must override the <see cref="BuildExpression"/> method in order to provide a custom behavior.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public abstract class SecurityUser<TSource> : SecurityUser, IQueryExpression<TSource>
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
        public static implicit operator Expression<Func<TSource, bool>>([NotNull] SecurityUser<TSource> criteria)
             => criteria?.Expression() ?? PredicateBuilder.New<TSource>();

        [SuppressMessage("Usage", "CA2225:Les surcharges d'opérateur offrent d'autres méthodes nommées",
            Justification = "<En attente>")]
        public static implicit operator Func<TSource, bool>(SecurityUser<TSource> criteria)
            => criteria?.Expression().Compile() ?? PredicateBuilder.New<TSource>();
    }
}
