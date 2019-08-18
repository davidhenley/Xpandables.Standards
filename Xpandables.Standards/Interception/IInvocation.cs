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

namespace System.Interception
{
    /// <summary>
    /// Provides the structure for an interception event.
    /// </summary>
    public interface IInvocation
    {
        /// <summary>
        /// Contains the arguments (position in signature, names and values) with which the method has been invoked.
        /// This argument is provided only for target element with parameters.
        /// </summary>
        IEnumerable<Parameter> Arguments { get; }

        /// <summary>
        /// Gets the exception handled on executing a method.
        /// You can edit this value in order to return a custom exception or null.
        /// If you set this value to null, the process will resume normally and
        /// take care to provide a <see cref="ReturnValue"/> if necessary.
        /// </summary>
        Optional<Exception> Exception { get; }

        /// <summary>
        /// Gets the executed method return value, only provided for non-void method and when no exception handled.
        /// </summary>
        Optional<object> ReturnValue { get; }

        /// <summary>
        /// Get the elapsed time execution in milliseconds for the underlying method.
        /// </summary>
        long ElapsedTime { get; }

        /// <summary>
        /// Sets the exception value.
        /// If you set this value to null, the process will resume normally and
        /// take care to provide a <see cref="ReturnValue" /> if necessary.
        /// </summary>
        /// <param name="exception">The exception value.</param>
        /// <returns>The current instance with exception value.</returns>
        IInvocation WithException(Exception exception);

        /// <summary>
        /// Sets the executed method return value, only for non-void method.
        /// Be aware to match the return value type.
        /// Otherwise it will throw an exception.
        /// </summary>
        /// <param name="returnValue">The return value to be used.</param>
        /// <returns>The current instance with return value.</returns>
        IInvocation WithReturnValue(object returnValue);

        /// <summary>
        /// Executes the underlying method.
        /// </summary>
        void Proceed();
    }
}