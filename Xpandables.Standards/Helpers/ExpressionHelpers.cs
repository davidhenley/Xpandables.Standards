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

namespace System.Linq.Expressions
{
    /// <summary>
    /// Extensions methods for <see cref="System.Linq.Expressions.Expression{TDelegate}"/>.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Returns the member name from the expression.
        /// The expression delegate is <see langword="nameof"/>, otherwise the result is null.
        /// </summary>
        /// <typeparam name="T">The data source type.</typeparam>
        /// <param name="nameOfExpression">The expression delegate for the property : <see langword="nameof"/>
        /// with delegate expected.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="nameOfExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="nameOfExpression"/> is
        /// not a <see cref="ConstantExpression"/>.</exception>
        public static string GetMemberNameForExpression<T>(this Expression<Func<T, string>> nameOfExpression)
        {
            if (nameOfExpression is null) throw new ArgumentNullException(nameof(nameOfExpression));

            return nameOfExpression.Body is ConstantExpression constantExpression
                ? constantExpression.Value.ToString()
                : throw new ArgumentException(
                    $"The parameter {nameof(nameOfExpression)} is not a {nameof(ConstantExpression)}.");
        }

        /// <summary>
        /// Returns the member name from the expression if found, otherwise returns null.
        /// </summary>
        /// <typeparam name="T">The data source type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member name.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is null.</exception>
        public static string GetMemberNameForExpression<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression is null) throw new ArgumentNullException(nameof(propertyExpression));

            return (propertyExpression.Body as MemberExpression
                ?? ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression)
                ?.Member.Name ??
                throw new ArgumentException(
                    $"The parameter {nameof(propertyExpression)} is not a {nameof(MemberExpression)}.");
        }

        /// <summary>
        /// Creates a property or field access-or expression.
        /// </summary>
        /// <typeparam name="T">The data source type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyOrFieldName">The name of the property or field.</param>
        /// <returns>An expression tree.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyOrFieldName"/> is null.</exception>
        /// <exception cref="ArgumentException">No property or field named propertyOrFieldName is
        /// defined in expression.Type or its base types.</exception>
        public static Expression<Func<T, TProperty>> CreateAccessorFor<T, TProperty>(this string propertyOrFieldName)
        {
            if (propertyOrFieldName is null) throw new ArgumentNullException(nameof(propertyOrFieldName));

            var paramExpr = Expression.Parameter(typeof(T));
            var bodyExpr = Expression.PropertyOrField(paramExpr, propertyOrFieldName);
            return Expression.Lambda<Func<T, TProperty>>(bodyExpr, new ParameterExpression[] { paramExpr });
        }

        /// <summary>
        /// Replaces the expression elements with the <paramref name="replaceWith"/> using
        /// the <paramref name="searchFor"/> expressions.
        /// </summary>
        /// <param name="expression">The expression to be replaced.</param>
        /// <param name="searchFor">The expression to be searched.</param>
        /// <param name="replaceWith">The expression to replace with.</param>
        /// <returns><see cref="Expression"/> instance with replaced value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> or
        /// <paramref name="searchFor"/> or <paramref name="replaceWith"/>
        /// can not be null.</exception>
        public static Expression Replace(this Expression expression, Expression searchFor, Expression replaceWith)
            => new ExpressionVisitorReplace(searchFor, replaceWith).Visit(expression);

        /// <summary>
        /// Replaces the expression elements with the <paramref name="replaceWith"/> using
        /// the <paramref name="searchFor"/> expressions.
        /// </summary>
        /// <param name="expression">The expression to be replaced.</param>
        /// <param name="searchFor">The collection of expressions to be searched.</param>
        /// <param name="replaceWith">The collection of expressions to replace with.</param>
        /// <returns><see cref="Expression"/> instance with replaced value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> or <paramref name="searchFor"/>
        /// or <paramref name="replaceWith"/>
        /// can not be null.</exception>
        public static Expression ReplaceAll(this Expression expression, Expression[] searchFor, Expression[] replaceWith)
        {
            var _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            var _searchFor = searchFor ?? throw new ArgumentNullException(nameof(searchFor));
            var _replaceWith = replaceWith ?? throw new ArgumentNullException(nameof(replaceWith));

            for (int i = 0; i < searchFor.Length; i++)
            {
                _expression = Replace(_expression, _searchFor[i], _replaceWith[i]);
            }

            return _expression;
        }
    }
}