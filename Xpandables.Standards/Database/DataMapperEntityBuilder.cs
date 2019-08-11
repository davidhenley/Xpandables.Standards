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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xpandables.Database
{
    /// <summary>
    ///
    /// </summary>
    public sealed class DataMapperEntityBuilder : Explicit<IDataMapperEntityBuilder>, IDataMapperEntityBuilder
    {
        IDataMapperEntity<T> IDataMapperEntityBuilder.Build<T>() => DoBuild<T>(typeof(T));

        IDataMapperEntity IDataMapperEntityBuilder.Build(Type type) => DoBuild(type);

        private static IDataMapperEntity<T> DoBuild<T>(Type type) where T : class, new()
        {
            var localType = type ?? throw new ArgumentNullException(nameof(type));

            if (localType.IsEnumerable())
                localType = localType.GetGenericArguments()[0];
            else if (localType.IsNullable())
                localType = Nullable.GetUnderlyingType(localType);

            var keyAttr = localType.GetAttribute<DataMapperUniqueKeyAttribute>().ToOptional();
            var keys = keyAttr.Map(attr => attr.Keys).Reduce(() => Array.Empty<string>());

            var properties = localType.GetProperties().Select(p => BuildProperty<T>(p, keys));

            return new DataMapperEntity<T>(properties);
        }

        private static IDataMapperProperty<T> BuildProperty<T>(PropertyInfo propertyInfo, string[] identities)
            where T : class, new()
        {
            var property = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            var keys = identities ?? throw new ArgumentNullException(nameof(identities));

            var ownerAttr = property.DeclaringType.GetAttribute<DataMapperPrefixAttribute>().ToOptional();
            var propertyAttr = property.GetAttribute<DataMapperPrefixAttribute>().ToOptional();
            var nameAttr = property.GetAttribute<DataMapperNameAttribute>().ToOptional();

            var dataPrefix = propertyAttr.Map(attr => attr.Prefix).ReduceOptional(() => ownerAttr.Map(attr => attr.Prefix));
            var dataName = nameAttr.Map(attr => attr.Name).Reduce(() => property.Name);
            var isIdentity = keys.Any(k => k == property.Name);
            var type = property.PropertyType;

            return dataPrefix
                    .Map(value => new DataMapperProperty<T>(property.Name, value, dataName, type).AllowIdentity(isIdentity))
                    .Reduce(() => new DataMapperProperty<T>(property.Name, dataName, type).AllowIdentity(isIdentity))
                    .OfType<IDataMapperProperty<T>>();
        }

        private static IDataMapperEntity DoBuild(Type type)
        {
            var localType = type ?? throw new ArgumentNullException(nameof(type));

            if (localType.IsEnumerable())
                localType = localType.GetGenericArguments()[0];
            else if (localType.IsNullable())
                localType = Nullable.GetUnderlyingType(localType);

            var keyAttr = localType.GetAttribute<DataMapperUniqueKeyAttribute>().ToOptional();
            var keys = keyAttr.Map(attr => attr.Keys).Reduce(() => Array.Empty<string>());

            var properties = localType.GetProperties().Select(p => BuildProperty(p, keys));

            return new DataMapperEntity(properties);
        }

        private static IDataMapperProperty BuildProperty(PropertyInfo propertyInfo, string[] identities)
        {
            var property = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            var keys = identities ?? throw new ArgumentNullException(nameof(identities));

            var ownerAttr = property.DeclaringType.GetAttribute<DataMapperPrefixAttribute>().ToOptional();
            var propertyAttr = property.GetAttribute<DataMapperPrefixAttribute>().ToOptional();
            var nameAttr = property.GetAttribute<DataMapperNameAttribute>().ToOptional();

            var dataPrefix = propertyAttr.Map(attr => attr.Prefix).ReduceOptional(() => ownerAttr.Map(attr => attr.Prefix));
            var dataName = nameAttr.Map(attr => attr.Name).Reduce(() => property.Name);
            var isIdentity = keys.Any(k => k == property.Name);
            var type = property.PropertyType;

            return dataPrefix
                    .Map(value => new DataMapperProperty(property.Name, value, dataName, type).AllowIdentity(isIdentity))
                    .Reduce(() => new DataMapperProperty(property.Name, dataName, type).AllowIdentity(isIdentity))
                    .OfType<IDataMapperProperty>();
        }
    }
}
