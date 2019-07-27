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

using System.Runtime.CompilerServices;

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
        /// <param name="callerClassName"></param>
        /// <param name="callerLine"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="contractMethodName"></param>
        /// <param name="message">The default exception message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public Contract(
            T value,
            Predicate<T> predicate,
            string callerClassName,
            int callerLine,
            string callerMemberName,
            [CallerMemberName] string contractMethodName = "",
            string message = "")
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _value = value;
            ContractEvent = new ContractEventArgs<T>(
                callerClassName, callerLine, callerMemberName, value, contractMethodName, message);
        }

        internal ContractEventArgs ContractEvent { get; }

        private bool IsValid => _predicate.Invoke(_value);

        private readonly T _value;
        private readonly Predicate<T> _predicate;

        /// <summary>
        /// Returns the specified value replacing the actual one when the contract failed.
        /// </summary>
        /// <param name="value">The value to be returned.</param>
        /// <returns>The specified value of <typeparamref name="T" /> type.</returns>
        public T Return(T value) => IsValid ? _value : value;

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

            return IsValid ? _value : valueProvider();
        }

        /// <summary>
        /// Throws the specified type of exception when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        public T ThrowException<TException>() where TException : Exception, new()
            => IsValid ? _value : throw this.ExceptionBuilder<T, TException>();

        /// <summary>
        /// Throws the specified type of exception from builder when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="exceptionBuilder">A delegate to build an instance of the expected exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionBuilder"/> is null.</exception>
        public T ThrowException<TException>(Func<T, TException> exceptionBuilder) where TException : Exception, new()
        {
            if (exceptionBuilder is null)
                throw new ArgumentNullException(nameof(exceptionBuilder));

            return IsValid ? _value : throw exceptionBuilder(_value);
        }

        /// <summary>
        /// Throws the specified type of exception from builder with the message when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="message">An exception message.</param>
        /// <param name="exceptionBuilder">A delegate to build an instance of the expected exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionBuilder"/> is null.</exception>
        public T ThrowException<TException>(string message, Func<T, string, TException> exceptionBuilder)
            where TException : Exception, new()
        {
            if (exceptionBuilder is null)
                throw new ArgumentNullException(nameof(exceptionBuilder));

            if (message is null)
                throw new ArgumentNullException(nameof(message));

            return IsValid ? _value : throw exceptionBuilder(_value, message);
        }

        /// <summary>
        /// Throws the specified type of exception with the message when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="message">An exception message.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T ThrowException<TException>(string message) where TException : Exception, new()
            => IsValid ? _value : throw this.ExceptionBuilder<T, TException>(message);

        /// <summary>
        /// Throws the specified type of exception with the message when the contract failed.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="message">An exception message.</param>
        /// <param name="innerException">An inner exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="innerException"/> is null.</exception>
        public T ThrowException<TException>(string message, Exception innerException) where TException : Exception, new()
            => IsValid ? _value : throw this.ExceptionBuilder<T, TException>(message, innerException);

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/>when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentNullException"/>.</returns>
        public T ThrowArgumentNullException()
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentNullException>(ContractEvent.Message);

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <returns>An <see cref="ArgumentNullException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T ThrowArgumentNullException(string message)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentNullException>(message);

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the message and inner exception when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <param name="innerException">An inner exception.</param>
        /// <returns>An <see cref="ArgumentNullException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="innerException"/> is null.</exception>
        public T ThrowArgumentNullException(string message, Exception innerException)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentNullException>(message, innerException);

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentException"/>.</returns>
        public T ThrowArgumentException()
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentException>();

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <returns>An <see cref="ArgumentException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T ThrowArgumentException(string message)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentException>(message);

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <param name="innerException">An inner exception.</param>
        /// <returns>An <see cref="ArgumentException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="innerException"/> is null.</exception>
        public T ThrowArgumentException(string message, Exception innerException)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentException>(message, innerException);

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with when the contract failed.
        /// </summary>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/>.</returns>
        public T ThrowArgumentOutOfRangeException()
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentOutOfRangeException>(ContractEvent.Message);

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T ThrowArgumentOutOfRangeException(string message)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentOutOfRangeException>(message);

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <param name="innerException">An inner exception.</param>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="innerException"/> is null.</exception>
        public T ThrowArgumentOutOfRangeException(string message, Exception innerException)
            => IsValid ? _value : throw this.ExceptionBuilder<T, ArgumentOutOfRangeException>(message, innerException);

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> with the message when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <returns>An <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T ThrowInvalidOperationException(string message)
            => IsValid
                ? _value
                : throw this.ExceptionBuilder<T, InvalidOperationException>(message ?? "The operation failed to execute.");

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> with the message and inner exception when the contract failed.
        /// </summary>
        /// <param name="message">An exception message.</param>
        /// <param name="innerException">The handled exception.</param>
        /// <returns>An <see cref="InvalidOperationException"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="innerException"/> is null.</exception>
        public T ThrowInvalidOperationException(string message, Exception innerException)
            => IsValid
                ? _value
                : throw this.ExceptionBuilder<T, InvalidOperationException>(
                    message ?? "The operation failed to execute. See inner exception",
                    innerException);
    }
}