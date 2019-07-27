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
    /// Visitor allows you to add new behaviors to an existing object without changing the object structure.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// Declares a Visit operation.
        /// </summary>
        /// <param name="element">Element to be visited, must implement <see cref="IVisitable"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Visit(object element);

        /// <summary>
        /// Determines the order for the underlying object.
        /// </summary>
        int Order { get; }
    }

    /// <summary>
    /// Allows an application author to apply the visitor pattern : The generic Visitor definition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public interface IVisitor<in TElement> : IVisitor
        where TElement : class, IVisitable
    {
        /// <summary>
        /// Declares a Visit operation.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Visit(TElement element);
    }
}