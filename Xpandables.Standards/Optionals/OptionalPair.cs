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
    /// <summary>
    /// Provides with pair optional values.
    /// </summary>
    public sealed class OptionalPair<TL, TR>
    {
        internal OptionalPair(TL left, TR right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        /// <summary>
        /// Gets the left value.
        /// </summary>
        public TL Left { get; }

        /// <summary>
        /// Gets the right value.
        /// </summary>
        public TR Right { get; }
    }

}