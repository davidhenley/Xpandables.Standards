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
using System.Reflection;

namespace System.Design.Linq
{
    /// <summary>
    /// Refer to http://www.albahari.com/nutshell/linqkit.html and http://tomasp.net/blog/linq-expand.aspx for more information.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns wrapper that automatically expands expressions using a default QueryOptimizer.
        /// </summary>
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            return AsExpandable(query, OptimizerExpression.QueryOptimizer);
        }

        /// <summary>
        /// Returns wrapper that automatically expands expressions using a custom QueryOptimizer.
        /// </summary>
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query, Func<Expression, Expression> queryOptimizer)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (queryOptimizer is null) throw new ArgumentNullException(nameof(queryOptimizer));

            if (query is ExpandableQuery<T>) return query;
            return ExpandableQueryFactory<T>.Create(query, queryOptimizer);
        }

        /// <summary>Expands expression.</summary>
        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        /// <summary>Expands expression.</summary>
        public static Expression Expand<TDelegate>(this ExpressionStarter<TDelegate> expr)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return new ExpressionExpander().Visit(expr);
        }

        /// <summary>Expands expression.</summary>
        public static Expression Expand(this Expression expr)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return new ExpressionExpander().Visit(expr);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke();
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, TResult>(
            this Expression<Func<T1, TResult>> expr,
            T1 arg1)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> expr,
            T1 arg1,
            T2 arg2)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11,
            T12 arg12)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11,
            T12 arg12,
            T13 arg13)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11,
            T12 arg12,
            T13 arg13,
            T14 arg14)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                                         arg13, arg14);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11,
            T12 arg12,
            T13 arg13,
            T14 arg14,
            T15 arg15)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                                         arg13, arg14, arg15);
        }

        /// <summary>Compile and invoke.</summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expr,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8,
            T9 arg9,
            T10 arg10,
            T11 arg11,
            T12 arg12,
            T13 arg13,
            T14 arg14,
            T15 arg15,
            T16 arg16)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                                         arg13, arg14, arg15, arg16);
        }

        private static class ExpandableQueryFactory<T>
        {
            public static readonly Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>> Create;

#pragma warning disable CA1810 // Initialize reference type static fields inline
            static ExpandableQueryFactory()
#pragma warning restore CA1810 // Initialize reference type static fields inline
            {
                if (!typeof(T).GetTypeInfo().IsClass)
                {
                    Create = (query, optimizer) => new ExpandableQuery<T>(query, optimizer);
                    return;
                }

                Type queryType = typeof(IQueryable<T>);
                Type optimizerType = typeof(Func<Expression, Expression>);

                var ctorInfo = typeof(ExpandableQueryOfClass<>)
                    .MakeGenericType(typeof(T))
                    .GetConstructor(new[] { queryType, optimizerType });

                var queryParam = Expression.Parameter(queryType);
                var optimizerParam = Expression.Parameter(optimizerType);

                var newExpr = Expression.New(ctorInfo, queryParam, optimizerParam);
                var createExpr = Expression.Lambda<Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>>>(
                    newExpr,
                    queryParam,
                    optimizerParam);

                Create = createExpr.Compile();
            }
        }
    }
}