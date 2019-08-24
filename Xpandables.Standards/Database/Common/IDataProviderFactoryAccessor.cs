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

using System.Data.Common;
using System.Reflection;

namespace System.Design.Database.Common
{
    /// <summary>
    /// Allows an application author to access a data provider factory from a provider type.
    /// <para>Contains default implementation.</para>
    /// </summary>
    public interface IDataProviderFactoryAccessor
    {
        /// <summary>
        /// Returns an instance of the data provider factory matching the specified type.
        /// </summary>
        /// <param name="providerType">The provider type to find factory.</param>
        /// <returns>An instance of <see cref="DbProviderFactory"/> if found, otherwise an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType)
        {
            if (providerType is null) throw new ArgumentNullException(nameof(providerType));

            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty;

            return providerType.ProviderFactoryTypeName
                .TypeFromString(providerType.DisplayName)
                .MapOptional(type => type.TypeInvokeMember("Instance", flags, null, type, null))
                .Cast<DbProviderFactory>();
        }
    }
}
