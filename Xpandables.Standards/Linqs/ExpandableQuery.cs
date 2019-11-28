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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace System.Design.Linq
{
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Les identificateurs doivent avoir un suffixe correct",
        Justification = "<En attente>")]
    public class ExpandableQuery<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        public ExpandableQuery(IQueryable<T> innerQuery, Func<Expression, Expression> queryOptimizer)
        {
            if (queryOptimizer is null) throw new ArgumentNullException(nameof(queryOptimizer));

            InnerQuery = innerQuery ?? throw new ArgumentNullException(nameof(innerQuery));
            InnerQueryProvider = new ExpandableQueryProvider<T>(this, queryOptimizer);
        }

        internal IQueryable<T> InnerQuery { get; }
        private ExpandableQueryProvider<T> InnerQueryProvider { get; }
        public Type ElementType => typeof(T);
        public Expression Expression => InnerQuery.Expression;
        public IQueryProvider Provider => InnerQueryProvider;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            if (InnerQuery is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator(cancellationToken);

            throw new InvalidOperationException(ErrorMessageResources.LinqQueryDontImplementIAsyncEnumeratorAccessor);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        /// <summary>
        /// IQueryable string presentation.
        /// </summary>
        public override string ToString() => InnerQuery.ToString();
    }
}