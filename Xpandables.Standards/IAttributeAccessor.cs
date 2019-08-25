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

using System.Reflection;

namespace System
{
    /// <summary>
    /// Allows an application author to access the <see cref="Attribute"/> from a specific type.
    /// <para>Contains default implementation.</para>
    /// </summary>
    public interface IAttributeAccessor
    {
        /// <summary>
        /// Returns the found attribute of type <typeparamref name="TAttribute"/> from the type.
        /// Otherwise returns an empty optional.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An optional instance that may be contains the found attribute.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        Optional<TAttribute> GetAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return type.GetCustomAttribute<TAttribute>();
        }
    }
}
