﻿/************************************************************************************************************
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
    /// Allows an application author to apply the visitor pattern using composition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public interface ICompositeVisitor<in TElement> : ICompositeVisitor
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

    /// <summary>
    /// Allows an application author to apply the visitor pattern using composition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface ICompositeVisitor
    {
        /// <summary>
        /// Declares a Visit operation.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        void Visit(object element);
    }
}