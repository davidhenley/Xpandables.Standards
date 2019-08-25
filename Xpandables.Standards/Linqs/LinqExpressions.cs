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

using System.Linq.Expressions;

namespace System.Design.Linq
{
    /// <summary>
    /// Another good idea by Tomas Petricek.
    /// See http://tomasp.net/blog/dynamic-linq-queries.aspx for information on how it's used.
    /// </summary>
    public static class LinqExpressions
    {
        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<TResult>> Expr<TResult>(Expression<Func<TResult>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<T1, T2, TResult>> Expr<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
        public static Func<TResult> Func<TResult>(Func<TResult> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
		public static Func<T, TResult> Func<T, TResult>(Func<T, TResult> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
        public static Func<T1, T2, TResult> Func<T1, T2, TResult>(Func<T1, T2, TResult> expr)
        {
            if (expr is null)
                throw new ArgumentNullException(nameof(expr));

            return expr;
        }
    }
}