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

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Design.Linq
{
    /// <summary>
    /// ExpressionStarter{T} which eliminates the default 1=0 or 1=1 stub expressions
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public class ExpressionStarter<T>
    {
        internal ExpressionStarter()
            : this(false) { }

        internal ExpressionStarter(bool defaultExpression)
        {
            var expression = defaultExpression ? (f => true) : (Expression<Func<T, bool>>)(f => false);

            DefaultExpression = Optional<Expression<Func<T, bool>>>.Some(expression);
            _predicate = Optional<Expression<Func<T, bool>>>.Empty;
        }

        internal ExpressionStarter(Expression<Func<T, bool>> expression)
            : this(false)
            => _predicate = expression;

        /// <summary>The actual Predicate. It can only be set by calling Start.</summary>
        private Optional<Expression<Func<T, bool>>> Predicate => _predicate.ReduceOptional(() => DefaultExpression);

        // => (IsStarted || !UseDefaultExpression) ? _predicate : DefaultExpression;

        private Optional<Expression<Func<T, bool>>> _predicate;

        /// <summary>Determines if the predicate is started.</summary>
        public bool IsStarted => _predicate.Any();

        /// <summary> A default expression to use only when the expression is null </summary>
        public bool UseDefaultExpression => DefaultExpression.Any();

        /// <summary>The default expression</summary>
        public Optional<Expression<Func<T, bool>>> DefaultExpression { get; set; }

        /// <summary>Set the Expression predicate</summary>
        /// <param name="exp">The first expression</param>
        public Expression<Func<T, bool>> Start(Expression<Func<T, bool>> exp)
        {
            if (IsStarted)
                throw new InvalidOperationException(ErrorMessageResources.LinqPredicateAlreadyStarted);

            return _predicate = exp;
        }

        /// <summary>Or</summary>
        public Expression<Func<T, bool>> Or([NotNull] Expression<Func<T, bool>> expr2)
        {
            if (expr2 is null) throw new ArgumentNullException(nameof(expr2));

            if (IsStarted)
                return _predicate = _predicate.Single().Or(expr2);

            return Start(expr2);
        }

        /// <summary>And</summary>
        public Expression<Func<T, bool>> And([NotNull] Expression<Func<T, bool>> expr2)
        {
            if (expr2 is null) throw new ArgumentNullException(nameof(expr2));

            if (IsStarted)
                return _predicate = _predicate.Single().And(expr2);

            return Start(expr2);
        }

        /// <summary> Show predicate string </summary>
        public override string ToString()
            => Predicate.Map(value => value.ToString()).Reduce(() => string.Empty);

#pragma warning disable CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator Expression<Func<T, bool>>([NotNull] ExpressionStarter<T> right)
            => right.ToOptional().MapOptional(value => value.Predicate);

        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator Func<T, bool>([NotNull] ExpressionStarter<T> right)
            => right.ToOptional().MapOptional<Func<T, bool>>(value => value.Predicate.Single().Compile());

        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator ExpressionStarter<T>([NotNull] Expression<Func<T, bool>> right)
            => new ExpressionStarter<T>(right);

#pragma warning restore CA2225 // Les surcharges d'opérateur offrent d'autres méthodes nommées
        /// <summary></summary>
        public Func<T, bool> Compile() { return Predicate.Single().Compile(); }

        /// <summary></summary>
        public Expression Body => Predicate.Single().Body;

        /// <summary></summary>
        public ExpressionType NodeType => Predicate.Single().NodeType;

        /// <summary></summary>
        public ReadOnlyCollection<ParameterExpression> Parameters => Predicate.Single().Parameters;

        /// <summary></summary>
        public Type Type => Predicate.Single().Type;

        /// <summary></summary>
        public string Name => Predicate.Single().Name;

        /// <summary></summary>
        public Type ReturnType => Predicate.Single().ReturnType;

        /// <summary></summary>
        public bool TailCall => Predicate.Single().TailCall;

        /// <summary></summary>
        public virtual bool CanReduce => Predicate.Single().CanReduce;
    }
}