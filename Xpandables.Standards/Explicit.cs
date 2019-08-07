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
    /// Provides with a property to access instance on explicit interface implementation.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    public abstract class Explicit<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Gets the underlying instance from an explicit implementation.
        /// </summary>
        protected TInterface Instance => this as TInterface;
    }
}
