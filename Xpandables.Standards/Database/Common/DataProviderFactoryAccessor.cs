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
using System.Globalization;
using System.Reflection;

namespace System.Design.Database.Common
{
    /// <summary>
    /// The default implementation to return data provider factory from provider type.
    /// </summary>
    public sealed class DataProviderFactoryAccessor : IDataProviderFactoryAccessor
    {
        public Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType)
        {
            if (providerType is null)
            {
                throw new ArgumentNullException(
                nameof(providerType),
                ErrorMessageResources.ArgumentExpected.StringFormat(
                    nameof(DataProviderFactoryAccessor.GetProviderFactory),
                    nameof(providerType)));
            }

            return GetProviderFactoryInstance(providerType)
                .Map(provider => provider as DbProviderFactory);

            static Optional<object> GetProviderFactoryInstance(DataProviderType dataProviderType)
            {
                return GetTypeFromTypeName(dataProviderType.ProviderFactoryTypeName)
                    .MapOptional(type => GetTypeFromInvokeMember(type, "Instance"))
                    .WhenEmptyOptional(() => GetAssemblyFromString(dataProviderType.DisplayName))
                    .MapOptional(ass =>
                        ass.GetExportedTypes().FirstOrEmpty(testc => testc.FullName == dataProviderType.ProviderFactoryTypeName))
                    .MapOptional(type => GetTypeFromInvokeMember(type, "Instance"));
            }

            static Optional<Type> GetTypeFromTypeName(string typeName)
            {
                try
                {
                    return Type.GetType(typeName, true, true);
                }
                catch (Exception exception) when (exception is TargetInvocationException
                                            || exception is TypeLoadException
                                            || exception is ArgumentException
                                            || exception is IO.FileNotFoundException
                                            || exception is IO.FileLoadException
                                            || exception is BadImageFormatException)
                {
                    return Optional<Type>.Exception(exception);
                }
            }

            static Optional<Assembly> GetAssemblyFromString(string assemblyName)
            {
                try
                {
                    return Assembly.Load(assemblyName);
                }
                catch (Exception exception) when (exception is ArgumentException
                                            || exception is IO.FileNotFoundException
                                            || exception is IO.FileLoadException
                                            || exception is BadImageFormatException)
                {
                    return Optional<Assembly>.Exception(exception);
                }
            }

            static Optional<object> GetTypeFromInvokeMember(Type type, string member)
            {
                try
                {
                    return type.InvokeMember(
                        member,
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                        null,
                        type,
                        null,
                        CultureInfo.InvariantCulture);
                }
                catch (Exception exception) when (exception is ArgumentException
                                            || exception is MethodAccessException
                                            || exception is MissingFieldException
                                            || exception is MissingMethodException
                                            || exception is TargetException
                                            || exception is AmbiguousMatchException
                                            || exception is InvalidOperationException)
                {
                    return Optional<object>.Exception(exception);
                }
            }
        }
    }
}
