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

using System.Linq;
using System.Linq.Expressions;

namespace System.Design.Linq
{
    /// <summary>
    /// Dot not directly use this class !
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Les identificateurs doivent avoir un suffixe correct",
        Justification = "<En attente>")]
    public class ExpandableQueryOfClass<T> : ExpandableQuery<T>
        where T : class
    {
        public ExpandableQueryOfClass(IQueryable<T> inner, Func<Expression, Expression> queryOptimizer)
            : base(inner, queryOptimizer) { }

        //public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        //    => ((IQueryable<T>)InnerQuery.Include(navigationPropertyPath)).AsExpandable();
    }
}