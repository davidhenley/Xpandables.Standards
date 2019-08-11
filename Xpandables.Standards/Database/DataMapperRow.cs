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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Xpandables.Database
{
    /// <summary>
    ///
    /// </summary>
    public sealed class DataMapperRow : IDataMapperRow
    {
        private readonly IDataMapperEntityBuilder _entityBuilder;
        static ConcurrentDictionary<string, IDataMapperEntity> dict = new ConcurrentDictionary<string, IDataMapperEntity>();
        public DataMapperRow(IDataMapperEntityBuilder enityBuilder)
        {
            _entityBuilder = enityBuilder ?? throw new ArgumentNullException(nameof(enityBuilder));
        }

        Optional<IDataMapperEntity<T>> IDataMapperRow.MapTo<T>(DataRow source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            var entity = _entityBuilder.Build<T>();
            entity.CreateEntity();
            entity.BuildSignature(properties
                => properties.Where(w => w.IsIdentity).Select(property => source[property.DataFullName]?.ToString()));

            if (dict.TryGetValue(entity.Signature, out var found))
                entity = found as IDataMapperEntity<T>;
            else dict.AddOrUpdate(entity.Signature, entity, (_, __) => entity);

            foreach (var property in entity.Properties)
            {
                if (!property.IsPrimitive)
                {
                    var addressEntity = _entityBuilder.Build(property.Type);
                    addressEntity.SetEntity(property.CreateInstance());

                    foreach (var addr in addressEntity.Properties)
                    {
                        if (source.Table.Columns.Contains(addr.DataFullName))
                        {
                            var data = source[addr.DataFullName];
                            addr.SetValue(addressEntity.Entity, data);
                        }
                    }

                    addressEntity.BuildSignature();

                    property.SetValue(entity.Entity, addressEntity.Entity);
                }
                else
                {
                    if (source.Table.Columns.Contains(property.DataFullName))
                    {
                        var data = source[property.DataFullName];
                        property.SetValue(entity.Entity, data);
                    }
                }
            }

            dict.AddOrUpdate(entity.Signature, entity, (_, __) => entity);

            return entity.ToOptional();
        }
    }
}
