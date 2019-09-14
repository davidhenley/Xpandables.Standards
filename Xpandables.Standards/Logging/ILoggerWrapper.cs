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

namespace System.Design.Logging
{
    /// <summary>
    /// Provides with logging definition for design.
    /// </summary>
    public interface ILoggerWrapper
    {
        /// <summary>
        /// Method executed before the body of the decorated method to which the decorator is applied.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="sender">The sender object.</param>
        /// <param name="argument">The optional argument.</param>
        void OnEntry<T>(IObjectDescriptor sender, Optional<T> argument);

        /// <summary>
        /// Method executed after the body of the decorated method to which the decorator is applied,
        /// but only when the method successfully returns (i.e. when no exception flies out the method.).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="sender">The sender object.</param>
        /// <param name="argument">The optional argument.</param>
        /// <param name="result">The optional result.</param>
        void OnSuccess<T, U>(IObjectDescriptor sender, Optional<T> argument, Optional<U> result);

        /// <summary>
        /// Method executed after the body of the decorated method to which the decorator is applied,
        /// in case that the method resulted with an exception.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="exception">The exception handled.</param>
        void OnException(IObjectDescriptor sender, Exception exception);

        /// <summary>
        /// Method executed after the body of the decorated method to which the decorator is applied,
        /// even when the method exists with an exception (this method is invoked from the finally block).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <typeparam name="U">The type of the result.</typeparam>
        /// <param name="sender">The sender object.</param>
        /// <param name="argument">The optional argument.</param>
        /// <param name="result">The optional result.</param>
        /// <param name="exception">The optional exception.</param>
        void OnExit<T, U>(IObjectDescriptor sender, Optional<T> argument, Optional<U> result, Optional<Exception> exception);
    }
}
