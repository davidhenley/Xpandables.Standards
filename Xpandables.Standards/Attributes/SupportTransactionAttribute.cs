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

namespace System
{
    /// <summary>
    /// Specifies that the decorated command/query will be applied transaction behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SupportTransactionAttribute : Attribute
    {
        /// <summary>
        /// Defines the <see cref="SupportTransactionAttribute"/> with the specified arguments.
        /// </summary>
        /// <param name="transactionScopeOption">Options for creating transaction scope.</param>
        /// <param name="transactionScopeAsyncFlowOption">Options for creating transaction flow across thread.</param>
        /// <param name="isolationLevel">The isolation level of transaction.</param>
        /// <param name="timeOut">The timeout period for the transaction if necessary.</param>
        public SupportTransactionAttribute(
            TransactionScopeOption transactionScopeOption,
            TransactionScopeAsyncFlowOption transactionScopeAsyncFlowOption,
            IsolationLevel isolationLevel,
            int? timeOut)
        {
            TransactionScopeOption = transactionScopeOption;
            TransactionScopeAsyncFlowOption = transactionScopeAsyncFlowOption;
            IsolationLevel = isolationLevel;
            TimeOut = timeOut;
        }

        /// <summary>
        /// Returns the <see cref=" TransactionScope"/> matching the attribute.
        /// </summary>
        /// <returns></returns>
        public TransactionScope GetTransactionScope()
            => new TransactionScope(
                TransactionScopeOption,
                TimeOut.GetValueOrDefault(0) > 0
                    ? new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel,
                        Timeout = TimeSpan.FromSeconds(TimeOut.GetValueOrDefault())
                    }
                    : new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel
                    },
                TransactionScopeAsyncFlowOption);

        public TransactionScopeOption TransactionScopeOption { get; }
        public TransactionScopeAsyncFlowOption TransactionScopeAsyncFlowOption { get; }
        public IsolationLevel IsolationLevel { get; }
        public int? TimeOut { get; }
    }
}
