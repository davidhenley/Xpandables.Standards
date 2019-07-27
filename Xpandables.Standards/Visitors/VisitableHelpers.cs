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
    /// Helpers for <see cref="IVisitable"/>.
    /// </summary>
    public static class VisitableHelpers
    {
        /// <summary>
        /// Defines the Accept operation with <see cref="ICompositeVisitor"/>.
        /// </summary>
        /// <param name="visitable">The visitable to act on.</param>
        /// <param name="visitor">The visitor to be applied on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="visitable"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static void Accept(this IVisitable visitable, ICompositeVisitor visitor)
        {
            if (visitable is null) throw new ArgumentNullException(nameof(visitable));
            if (visitor is null) throw new ArgumentNullException(nameof(visitor));

            visitor.Visit(visitable);
        }
    }
}