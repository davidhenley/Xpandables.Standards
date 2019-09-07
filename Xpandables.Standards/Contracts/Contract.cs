/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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

using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Allows an application author to check constructor arguments.
    /// </summary>
    /// <typeparam name="T">Type of the value to act on.</typeparam>
    public class Contract<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Contract{T}"/> with the value to be checked and the
        /// predicate to be applied on this value.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="predicate">The predicate to be applied.</param>
        /// <param name="message">The default exception message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public Contract(
            T value,
            Predicate<T> predicate,
            string message)
        {
            Value = value;
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Gets the exeception message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the value to check.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the criteria to be applied.
        /// </summary>
        public Predicate<T> Predicate { get; }

        /// <summary>
        /// Determine whether or not the value matchs the criteria.
        /// </summary>
        public bool IsValid => Predicate.Invoke(Value);

        /// <summary>
        /// Returns the specified value replacing the actual one when the contract failed.
        /// </summary>
        /// <param name="newValue">The value to be returned.</param>
        /// <returns>The specified value of <typeparamref name="T" /> type.</returns>
        public T Return(T newValue) => IsValid ? Value : newValue;

        /// <summary>
        /// Returns the specified value replacing the actual one when the contract failed.
        /// Be aware to handle any kind of exception.
        /// </summary>
        /// <param name="valueProvider">The delegate that will be used to provide the new value.</param>
        /// <returns>A new value returned by the delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="valueProvider"/> is null.</exception>
        public T Return(Func<T> valueProvider)
        {
            if (valueProvider is null)
                throw new ArgumentNullException(nameof(valueProvider));

            return IsValid ? Value : valueProvider();
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/>when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentNullException"/> if failed, otherwise the value.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="Predicate"/> failed.</exception>
        public T ThrowArgumentNullException()
            => IsValid
                ? Value
                : throw this.BuildException(ContractExceptionBuilders.BuildArgumentNullException<T>());

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentException"/>.</returns>
        public T ThrowArgumentException()
            => IsValid
                ? Value
                : throw this.BuildException(ContractExceptionBuilders.BuildArgumentException<T>());

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/>.</returns>
        public T ThrowArgumentOutOfRangeException()
            => IsValid
                ? Value
                : throw this.BuildException(ContractExceptionBuilders.BuildArgumentOutOfRangeException<T>());

        /// <summary>
        /// Throws the specified type of exception when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        public T ThrowException<TException>() where TException : Exception
            => IsValid
                ? Value
                : throw this.BuildException(ContractExceptionBuilders.BuildCustomException<T, TException>());

        /// <summary>
        /// Throws the specified type of exception from builder when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="exceptionBuilder">A delegate to build an instance of the expected exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionBuilder"/> is null.</exception>
        public T ThrowException<TException>(Func<Contract<T>, TException> exceptionBuilder)
            where TException : Exception
        {
            if (exceptionBuilder is null)
                throw new ArgumentNullException(nameof(exceptionBuilder));

            return IsValid
                ? Value
                : throw this.BuildException(exceptionBuilder);
        }
    }
}