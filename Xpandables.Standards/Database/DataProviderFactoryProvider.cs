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
using System.Data.Common;
using System.Reflection;

namespace Xpandables.Database
{
    /// <summary>
    /// The default implementation to return data provider factory from provider type.
    /// </summary>
    public sealed class DataProviderFactoryProvider : IDataProviderFactoryProvider
    {
        public Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType)
        {
            if (providerType is null) throw new ArgumentNullException(nameof(providerType));

            return GetproviderFactoryInstance()
                .Map(result => result as DbProviderFactory);

            Optional<object> GetproviderFactoryInstance()
                => Type.GetType(providerType.ProviderFactoryTypeName, false, true).ToOptional()
                        .Map(type => TypeInvokeMember(type, "Instance"))
                        .Reduce(() => AssemblyLoadFromString(providerType.DisplayName))
                        .Map(obj => obj as Assembly)
                        .MapOptional(ass => ass.GetExportedTypes()
                            .FirstOrEmpty(t => t.FullName == providerType.ProviderFactoryTypeName))
                        .Map(type => TypeInvokeMember(type, "Instance"));

            static Assembly AssemblyLoadFromString(string assemblyName)
            {
                try { return Assembly.Load(assemblyName); }
                catch { return default; }
            }

            static object TypeInvokeMember(Type type, string member)
            {
                try
                {
                    return type.InvokeMember(
                                member,
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                                null, type, null);
                }
                catch { return default; }
            }
        }
    }
}
