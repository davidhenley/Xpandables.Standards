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

using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace System.Design.Linq
{
    internal class ExpandableQueryProvider<T> : IQueryProvider, IAsyncQueryProvider
    {
        private readonly ExpandableQuery<T> _query;
        private readonly Func<Expression, Expression> _queryOptimizer;

        internal ExpandableQueryProvider(ExpandableQuery<T> query, Func<Expression, Expression> queryOptimizer)
        {
            _query = query;
            _queryOptimizer = queryOptimizer;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.
        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            var expanded = expression.Expand();
            var optimized = _queryOptimizer(expanded);
            return _query.InnerQuery.Provider.CreateQuery<TElement>(optimized).AsExpandable();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.Expand());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            var expanded = expression.Expand();
            var optimized = _queryOptimizer(expanded);
            return _query.InnerQuery.Provider.Execute<TResult>(optimized);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            var expanded = expression.Expand();
            var optimized = _queryOptimizer(expanded);
            return _query.InnerQuery.Provider.Execute(optimized);
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "RCS1047:Non-asynchronous method name should not end with 'Async'.", Justification = "<En attente>")]
        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expanded = expression.Expand();
            var optimized = _queryOptimizer(expanded);

            if (_query.InnerQuery.Provider is IAsyncQueryProvider asyncQueryProvider)
#pragma warning disable EF1001 // Internal EF Core API usage.
                return asyncQueryProvider.ExecuteAsync<TResult>(optimized, cancellationToken);
#pragma warning restore EF1001 // Internal EF Core API usage.

            return _query.InnerQuery.Provider.Execute<TResult>(optimized);
        }
    }
}