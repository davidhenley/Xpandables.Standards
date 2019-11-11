/************************************************************************************************************
 * Copyright (c) 2007-2019 Joseph Albahari, Tomas Petricek, Scott Smith
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Design.Linq
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    internal class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        private readonly Optional<Dictionary<ParameterExpression, Expression>> _replaceVars;

        internal ExpressionExpander()
        {
            _replaceVars = Optional<Dictionary<ParameterExpression, Expression>>.Empty();
        }

        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
        {
            _replaceVars = replaceVars;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));

            if (_replaceVars.Any() && _replaceVars.Single().ContainsKey(node))
                return _replaceVars.Single()[node];

            return base.VisitParameter(node);
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));

            var target = node.Expression;
            if (target is MemberExpression memberExpression) target = TransformExpr(memberExpression);
            if (target is ConstantExpression constantExpression) target = (Expression)constantExpression.Value;

            var lambda = (LambdaExpression)target;
            Dictionary<ParameterExpression, Expression> replaceVars =
                _replaceVars
                    .Map(dict => new Dictionary<ParameterExpression, Expression>(dict))
                    .WhenEmpty(() => new Dictionary<ParameterExpression, Expression>());

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                    replaceVars.Add(lambda.Parameters[i], Visit(node.Arguments[i]));
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(
                    ErrorMessageResources.LinqInvokeRecursivelyNotAllow,
                    exception);
            }

            return new ExpressionExpander(replaceVars).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));

            if (node.Method.Name == "Invoke" && node.Method.DeclaringType == typeof(LinqExtensions))
            {
                var target = node.Arguments[0];

                if (target is MemberExpression memberExpression) target = TransformExpr(memberExpression);
                if (target is ConstantExpression constantExpression) target = (Expression)constantExpression.Value;
                if (target is UnaryExpression unaryExpression) target = unaryExpression.Operand;

                if (target is LambdaExpression lambda)
                {
                    Dictionary<ParameterExpression, Expression> replaceVars =
                      _replaceVars
                          .Map(dict => new Dictionary<ParameterExpression, Expression>(dict))
                          .WhenEmpty(() => new Dictionary<ParameterExpression, Expression>());

                    try
                    {
                        for (int i = 0; i < lambda.Parameters.Count; i++)
                            replaceVars.Add(lambda.Parameters[i], Visit(node.Arguments[i + 1]));
                    }
                    catch (ArgumentException exception)
                    {
                        throw new InvalidOperationException(
                            ErrorMessageResources.LinqInvokeRecursivelyNotAllow,
                            exception);
                    }

                    return new ExpressionExpander(replaceVars).Visit(lambda.Body);
                }
            }

            // Expand calls to an expression's Compile() method:
            if (node.Method.Name == "Compile" && node.Object is MemberExpression memberExpression1)
            {
                var newExpr = TransformExpr(memberExpression1);
                if (newExpr != memberExpression1) return newExpr;
            }

            // Strip out any nested calls to AsExpandable():
            if (node.Method.Name == "AsExpandable" && node.Method.DeclaringType == typeof(LinqExtensions))
                return node.Arguments[0];

            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));

            return node.Member.DeclaringType?.Name.StartsWith("<>", StringComparison.InvariantCulture) == true
                    ? TransformExpr(node)
                    : base.VisitMember(node);
        }

        private Expression TransformExpr(MemberExpression member)
        {
            if (!(member.Member is FieldInfo field))
            {
                if (_replaceVars.Any() && member.Expression is ParameterExpression parameterExpression)
                {
                    if (_replaceVars.Single().ContainsKey(parameterExpression))
                        return base.VisitMember(member);
                }

                return member;
            }

            //Collapse captured outer variables
            if (member.Member.DeclaringType != null
                && (!member.Member.DeclaringType.GetTypeInfo().IsNestedPrivate
                    || !member.Member.DeclaringType.Name.StartsWith("<>", StringComparison.InvariantCulture)))
            // captured outer variable
            {
                return TryVisitExpressionFunc(member, field);
            }

            if (member.Expression is ConstantExpression expression)
            {
                if (expression.Value is null) return member;

                if (!expression.Value.GetType().GetTypeInfo().IsNestedPrivate
                    || !expression.Value.GetType().Name.StartsWith("<>", StringComparison.InvariantCulture))
                {
                    return member;
                }

                if (member.Member is FieldInfo fieldInfo)
                {
                    var fieldValue = fieldInfo.GetValue(expression.Value);
                    if (fieldValue is Expression expression1)
                        return Visit(expression1);
                }
            }

            return TryVisitExpressionFunc(member, field);
        }

        private Expression TryVisitExpressionFunc(MemberExpression member, FieldInfo fieldInfo)
        {
            var propertyInfo = member.Member as PropertyInfo;
            if (fieldInfo.FieldType.GetTypeInfo().IsSubclassOf(typeof(Expression))
                || (propertyInfo?.PropertyType.GetTypeInfo().IsSubclassOf(typeof(Expression)) == true))
            {
                return Visit(Expression.Lambda<Func<Expression>>(member).Compile()());
            }

            return member;
        }
    }
}