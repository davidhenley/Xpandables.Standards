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

using System.Design.Linq;
using System.Linq.Expressions;

namespace System.Design.Query
{
    /// <summary>
    /// This class is a helper that provides a default implementation for <see cref="IQueryExpression{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public class QueryExpression<TSource> : IQueryExpression<TSource>
        where TSource : class
    {
        public Expression<Func<TSource, bool>> Expression => BuildExpression();

        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        protected virtual Expression<Func<TSource, bool>> BuildExpression() => PredicateBuilder.New<TSource>();

#pragma warning disable CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
        public static implicit operator Expression<Func<TSource, bool>>(QueryExpression<TSource> criteria)
#pragma warning restore CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
#pragma warning disable CA1062 // Valider les arguments de méthodes publiques
             => criteria.Expression;
#pragma warning restore CA1062 // Valider les arguments de méthodes publiques

#pragma warning disable CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
        public static implicit operator Func<TSource, bool>(QueryExpression<TSource> criteria)
#pragma warning restore CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
#pragma warning disable CA1062 // Valider les arguments de méthodes publiques
            => criteria.Expression.Compile();
#pragma warning restore CA1062 // Valider les arguments de méthodes publiques
    }
}