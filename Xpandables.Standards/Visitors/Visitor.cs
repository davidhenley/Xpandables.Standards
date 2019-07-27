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

namespace System.Patterns
{
    /// <summary>
    /// The generic visitor definition implementation.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    [Serializable]
    public abstract class Visitor : IVisitor
    {
        /// <summary>
        /// Determines the zero-base order in which the visitor will be applied.
        /// The default value is zero.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// When overridden in derived class, this method will do the actual job of visiting the specified argument.
        /// The default behavior just do nothing.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual void Visit(object element) { }
    }

    /// <summary>
    /// The generic visitor definition implementation.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    [Serializable]
    public abstract class Visitor<TElement> : Visitor, IVisitor<TElement>
        where TElement : class, IVisitable
    {
        /// <summary>
        /// When overridden in derived class, this method will do the actual job of visiting the specified argument.
        /// The default behavior just do nothing.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual void Visit(TElement element) { }

        public override void Visit(object element) => Visit(element as TElement);
    }
}