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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace System
{
    /// <summary>
    ///  Contains static methods for representing program contracts that check for argument null or empty,
    ///  customs conditions and throws customs exceptions. If you don't want to use message,
    ///  set the value to <see langword="null"/> or <see langword="default"/>.
    ///  You can defines your own extension contract method to match your requirement.
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// Contains the event that being raised when exception is thrown.
        /// </summary>
        public static event EventHandler<ContractEventArgs> ExceptionHandler;

        private static void OnExceptionHandled(ContractEventArgs guardEvent)
            => ExceptionHandler?.Invoke(null, guardEvent);

        /// <summary>
        /// Checks whether the target value is null.
        /// </summary>
        /// <param name="actual">actual value.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNull</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNull</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNull</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNull<T>(
            this T actual,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            => new Contract<T>(actual, value => !EqualityComparer<T>.Default.Equals(value, default),
                callerClassName, callerLineNumber, callerMemberName, message: $"A value is expected for the specified argument.");

        /// <summary>
        /// Checks whether the target value is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="actual">actual value.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNull</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNull</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNull</param>
        /// <returns>An instance of <see cref="Contract{T}"/> where T is <see cref="string"/>.</returns>
        public static Contract<string> WhenNull(
            this string actual,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            => new Contract<string>(actual, value => !string.IsNullOrWhiteSpace(value),
                callerClassName, callerLineNumber, callerMemberName, message: $"A value is expected for the specified argument.");

        /// <summary>
        /// Checks whether the predicate is <see langword="true"/>.
        /// </summary>
        /// <param name="actual">actual value.</param>
        /// <param name="predicate">The predicate to be used.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenConditionFailed</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenConditionFailed</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenConditionFailed</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenConditionFailed<T>(
            this T actual, Predicate<T> predicate,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            => new Contract<T>(actual, predicate, callerClassName, callerLineNumber, callerMemberName);

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not greater than the <paramref name="comp"/>.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNotGreaterThan</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNotGreaterThan</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNotGreaterThan</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotGreaterThan<T>(
            this T actual, T comp,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                actual, value => value.CompareTo(comp) > 0, callerClassName, callerLineNumber, callerMemberName,
                message: $"The specified value '{actual}' must be greater than {comp}.");

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not greater than or equal to the <paramref name="comp"/>.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNotGreaterThanOrEqualTo</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNotGreaterThanOrEqualTo</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNotGreaterThanOrEqualTo</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotGreaterThanOrEqualTo<T>(
            this T actual, T comp,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(actual, value => value.CompareTo(comp) >= 0, callerClassName, callerLineNumber, callerMemberName,
                message: $"The specified value '{actual}' must be greater than or equal to {comp}.");

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not lower than the <paramref name="comp"/>.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNotLowerThan</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNotLowerThan</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNotLowerThan</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotLowerThan<T>(
            this T actual, T comp,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(actual, value => value.CompareTo(comp) < 0, callerClassName, callerLineNumber, callerMemberName,
                message: $"The specified value '{actual}' must be lower than {comp}.");

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not lower than or equal to the <paramref name="comp"/>.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNotLowerThanOrEqualTo</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNotLowerThanOrEqualTo</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNotLowerThanOrEqualTo</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotLowerThanOrEqualTo<T>(
            this T actual, T comp,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(actual, value => value.CompareTo(comp) <= 0, callerClassName, callerLineNumber, callerMemberName,
                message: $"The specified value '{actual}' must be lower than or equal to {comp}.");

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not in range of values provided.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="callerClassName">describe callerClassName parameter on WhenNotInRange</param>
        /// <param name="callerLineNumber">describe callerLineNumber parameter on WhenNotInRange</param>
        /// <param name="callerMemberName">describe callerMemberName parameter on WhenNotInRange</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotInRange<T>(
            this T actual, T min, T max,
            [CallerFilePath] string callerClassName = default,
            [CallerLineNumber] int callerLineNumber = default,
            [CallerMemberName] string callerMemberName = default)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(actual, value => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0,
                callerClassName, callerLineNumber, callerMemberName,
                message: $"The specified value '{actual}' must be in range of {min} to {max}.");

        /// <summary>
        /// This method is used to build the <typeparamref name="TException"/>.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="contract">The actual contract being executed.</param>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new instance of the expected exception type.</returns>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static TException ExceptionBuilder<T, TException>(this Contract<T> contract)
            where TException : Exception
            => contract.BuildException(Activator.CreateInstance<TException>);

        /// <summary>
        /// This method is used to build the <typeparamref name="TException"/> with an exception message.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="contract">The actual contract being executed.</param>
        /// <param name="message">The exception message.</param>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new instance of the expected exception type with the message.</returns>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static TException ExceptionBuilder<T, TException>(
            this Contract<T> contract, string message)
            where TException : Exception
            => contract.BuildException(() => Activator.CreateInstance(typeof(TException), new object[] { message }) as TException);

        /// <summary>
        /// This method is used to build the <typeparamref name="TException"/> with an exception message and inner exception.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <typeparam name="TException">Type of exception to build.</typeparam>
        /// <param name="contract">The actual contract being executed.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>A new instance of the expected exception type with the message and inner exception.</returns>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static TException ExceptionBuilder<T, TException>(
           this Contract<T> contract, string message, Exception innerException)
           where TException : Exception => contract.BuildException(()
               => Activator.CreateInstance(typeof(TException), new object[] { message, innerException }) as TException);

        private static TException BuildException<T, TException>(this Contract<T> contract, Func<TException> exceptionCreator)
            where TException : Exception
        {
            OnExceptionHandled(contract.ContractEvent);

            try
            {
                return exceptionCreator?.Invoke();
            }
            catch (Exception exception)
            {
                ExceptionDispatchInfo.Capture(
                    new InvalidOperationException(
                        $"Creating {typeof(TException)} failed. See inner exception.",
                        exception))
                    .Throw();

                // For compiler
                return default;
            }
        }
    }
}