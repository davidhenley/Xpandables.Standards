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

using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Helper class to implement <see cref="IObjectDescriptor"/>.
    /// </summary>
    /// <typeparam name="T">The type of the target</typeparam>
    public abstract class ObjectDescriptor<T> : IObjectDescriptor
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ObjectDescriptor{T}"/>.
        /// </summary>
        protected ObjectDescriptor()
        {
            Name = typeof(T).Name;
            Description = string.Empty;
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ObjectDescriptor{T}"/> with the specified one.
        /// </summary>
        /// <param name="objectDescriptor">The descriptor to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="objectDescriptor"/> is null.</exception>
        protected ObjectDescriptor(IObjectDescriptor objectDescriptor)
        {
            if (objectDescriptor is null) throw new ArgumentNullException(nameof(objectDescriptor));
            Name = objectDescriptor.Name;
            Description = objectDescriptor.Description;
            Properties = objectDescriptor.Properties;
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
