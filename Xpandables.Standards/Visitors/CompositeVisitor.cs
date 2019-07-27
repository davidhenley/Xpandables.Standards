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

using System.Collections.Generic;
using System.Linq;

namespace System.Patterns
{
    /// <summary>
    /// The composite visitor used to wrap all visitors for a specific type.
    /// </summary>
    /// <typeparam name="TElement">Type of the element to be visited</typeparam>
    [Serializable]
    public sealed class CompositeVisitor<TElement> : ICompositeVisitor<TElement>
        where TElement : class, IVisitable
    {
        private readonly IEnumerable<IVisitor<TElement>> _visitors;

        public CompositeVisitor(IEnumerable<IVisitor<TElement>> visitors)
            => _visitors = visitors;

        public void Visit(TElement element)
        {
            foreach (var visitor in _visitors.OrderBy(o => o.Order))
                element.Accept(visitor);
        }

        void ICompositeVisitor.Visit(object element) => Visit(element as TElement);
    }
}