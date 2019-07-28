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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Provides information for pagination.
    /// This class is serializable.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Index}, {Size}")]
    public sealed class Paging : IValidatableAttribute
    {
        /// <summary>
        /// Returns a new instance of <see cref="Paging"/> with the specified arguments.
        /// </summary>
        /// <param name="index">The one-based page index.</param>
        /// <param name="size">The one-based number of items per page.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/>
        /// or <paramref name="size"/> is negative.</exception>
        public Paging(int index, int size)
        {
            if (index <= 0)
                throw new ArgumentOutOfRangeException($"The parameter {nameof(index)} is lower or equal than zero.");
            if (size <= 0)
                throw new ArgumentOutOfRangeException($"The parameter {nameof(size)} is lower or equal than zero.");

            Index = index;
            Size = size;
        }

        /// <summary>
        /// Gets or sets the zero-based current page index.
        /// The default value is one.
        /// </summary>
        [Required, Range(1, int.MaxValue), DefaultValue(1)]
        public int Index { get; }

        /// <summary>
        /// Gets or sets the one-based number of items in a page.
        /// The default value is <see cref="int.MaxValue"/>.
        /// </summary>
        [Required, Range(1, int.MaxValue), DefaultValue(int.MaxValue)]
        public int Size { get; }
    }
}