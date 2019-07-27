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
#nullable disable
    /// <summary>
    /// Event that contains information when dealing with validation contract.
    /// </summary>
    public class ContractEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new contract event with supplied information.
        /// </summary>
        /// <param name="className">The path and name of the file.</param>
        /// <param name="lineNumber">The line number in the file.</param>
        /// <param name="memberName">The member that is calling.</param>
        /// <param name="value">The value being checked.</param>
        /// <param name="methodName">The contract method name being called.</param>
        /// <param name="message">The contract event message.</param>
        public ContractEventArgs(
            string className, int lineNumber, string memberName, object value, string methodName, string message)
        {
            ClassName = className;
            LineNumber = lineNumber;
            MemberName = memberName;
            Value = value;
            MethodName = methodName;
            Message = message;
        }

        /// <summary>
        /// Contains the contract event message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Contains the calling path and class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Contains the calling line number in the file.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Contains the calling member name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// The supplied value being checked.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Contains the contract method name being called.
        /// </summary>
        public string MethodName { get; }
    }

    /// <summary>
    /// Event that contains information when dealing with validation contract of specific type.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class ContractEventArgs<T> : ContractEventArgs
    {
        public ContractEventArgs(string className, int lineNumber, string memberName, T value, string methodName, string message)
            : base(className, lineNumber, memberName, value, methodName, message)
        {
            Value = value;
        }

        /// <summary>
        /// The supplied value being checked.
        /// </summary>
        public new T Value { get; }
    }
#nullable enable
}