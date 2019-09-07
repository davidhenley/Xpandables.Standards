﻿/************************************************************************************************************
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Xpandables
{
    /// <summary>
    /// Helper class to implement the <see cref="IDescriptor"/>.
    /// </summary>
    public abstract class Descriptor : IDescriptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Descriptor"/> with the name provided.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        protected Descriptor(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = string.Empty;
            Properties = ImmutableDictionary<string, object>.Empty;
        }

        /// <summary>
        /// Contains the name of the underlying instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Contains a description of the underlying instance.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Contains a collection of key/values pairs of the underlying instance.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties { get; }

    }
}
