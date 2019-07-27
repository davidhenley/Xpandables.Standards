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
    /// The base visitable definition implementation.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    [Serializable]
    public abstract class Visitable : IVisitable
    {
        /// <summary>
        /// When overridden in derived class, this method will accept the specified visitor.
        /// The default behavior just call the visit method of the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
        public virtual void Accept(IVisitor visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.Visit(this);
        }
    }
}