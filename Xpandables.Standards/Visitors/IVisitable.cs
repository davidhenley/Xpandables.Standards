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

namespace System.Patterns
{
    /// <summary>
    /// Defines an Accept operation that takes a visitor as an argument.
    /// Visitor design pattern allows you to add new behaviors to an existing object without changing the object structure.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Defines the Accept operation.
        /// </summary>
        /// <param name="visitor">The visitor to be applied on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Accept(IVisitor visitor);
    }
}