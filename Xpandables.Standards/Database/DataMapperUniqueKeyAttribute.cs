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

using System;

namespace Xpandables.Database
{
    /// <summary>
    /// Denotes one or more properties that uniquely identify the decorated class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DataMapperUniqueKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperUniqueKeyAttribute"/> with the collection of keys.
        /// </summary>
        /// <param name="keys">List of keys to be used to uniquely identify the decorated class.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="keys"/> is null or empty.</exception>
        public DataMapperUniqueKeyAttribute(params string[] keys)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));
            if (keys.Length <= 0)
                throw new ArgumentException("The collection of keys can not be empty.");

            Keys = keys;
        }

        /// <summary>
        /// Gets the collection of keys used to uniquely identify the decorated class.
        /// </summary>
        public string[] Keys { get; }
    }
}
