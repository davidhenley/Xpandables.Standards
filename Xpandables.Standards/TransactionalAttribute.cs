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

using System.Transactions;

namespace System
{
    /// <summary>
    /// Defines the attribute that it's used for applying Transaction Decorator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TransactionalAttribute : Attribute
    {
        private readonly TransactionScopeOption _transactionScopeOption;
        private readonly TransactionScopeAsyncFlowOption _transactionScopeAsyncFlowOption;
        private readonly IsolationLevel _isolationLevel;
        private readonly int _timeOut;

        /// <summary>
        /// Initializes a new instance of <see cref="TransactionalAttribute"/> with
        /// the specified transaction options and timeout period to be applied.
        /// </summary>
        /// <param name="transactionScopeOption">Options for creating transaction scope.</param>
        /// <param name="transactionScopeAsyncFlowOption">Options for creating transaction flow across thread.</param>
        /// <param name="isolationLevel">The isolation level of transaction.</param>
        /// <param name="timeOut">The timeout period for the transaction.</param>
        public TransactionalAttribute(
            TransactionScopeOption transactionScopeOption,
            TransactionScopeAsyncFlowOption transactionScopeAsyncFlowOption,
            IsolationLevel isolationLevel,
            int? timeOut)
        {
            _transactionScopeOption = transactionScopeOption;
            _transactionScopeAsyncFlowOption = transactionScopeAsyncFlowOption;
            _isolationLevel = isolationLevel;
            _timeOut = timeOut ?? -1;
        }

        /// <summary>
        /// Gets the transaction scope from the current attribute.
        /// </summary>
        public TransactionScope TransactionScope
            => new TransactionScope(
                _transactionScopeOption,
                _timeOut > 0
                    ? new TransactionOptions { IsolationLevel = _isolationLevel, Timeout = TimeSpan.FromSeconds(_timeOut) }
                    : new TransactionOptions { IsolationLevel = _isolationLevel },
                _transactionScopeAsyncFlowOption);
    }
}