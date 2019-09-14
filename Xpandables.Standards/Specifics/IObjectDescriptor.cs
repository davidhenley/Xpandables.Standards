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
    /// Provides information about an object.
    /// </summary>
    public interface IObjectDescriptor : IFluent
    {
        /// <summary>
        /// Contains the name of the underlying instance.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Contains a description of the underlying instance.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Contains a collection of key/values pairs information about the underlying instance.
        /// </summary>
        IReadOnlyDictionary<string, object> Properties { get; }
    }
}
