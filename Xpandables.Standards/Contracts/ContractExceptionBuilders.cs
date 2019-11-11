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

namespace System
{
    /// <summary>
    /// Provides with method contract for building exceptions.
    /// </summary>
    public static class ContractExceptionBuilders
    {
        internal static Func<Contract<T>, ArgumentNullException> BuildArgumentNullException<T>()
            => contract => new ArgumentNullException(contract.Message);

        internal static Func<Contract<T>, ArgumentException> BuildArgumentException<T>()
            => contract => new ArgumentException(contract.Message);

        internal static Func<Contract<T>, ArgumentOutOfRangeException> BuildArgumentOutOfRangeException<T>()
            => contract => new ArgumentOutOfRangeException(contract.Message);

        internal static Func<Contract<T>, InvalidOperationException> BuildInvalidOperationException<T>()
            => contract => new InvalidOperationException(contract.Message);

        /// <summary>
        /// Build an exception of the type-specific.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
        public static Func<Contract<T>, TException> BuildCustomException<T, TException>()
            where TException : Exception
            => contract => (TException)Activator.CreateInstance(typeof(TException), new object[] { contract.Message });

        /// <summary>
        /// Generic exception builder for contract.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of the exception to build.</typeparam>
        /// <param name="contract">The contract instance.</param>
        /// <param name="exceptionCreator">The exception builder.</param>
        /// <returns>An instance of <typeparamref name="TException"/> exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contract"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionCreator"/> is null.</exception>
        public static TException BuildException<T, TException>(
            this Contract<T> contract,
            Func<Contract<T>, TException> exceptionCreator)
            where TException : Exception
        {
            if (contract is null) throw new ArgumentNullException(nameof(contract));
            if (exceptionCreator is null) throw new ArgumentNullException(nameof(exceptionCreator));

            return exceptionCreator.Invoke(contract);
        }
    }
}
