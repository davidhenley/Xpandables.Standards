/************************************************************************************************************
 * Copyright (C) 2018 Francis-Black EWANE
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
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace System.GraphQL
{
    public sealed class TableObjectBuilder : ITableObjectBuilder
    {
        private readonly IColumnObjectBuilder _columnBuilder;
        private readonly INavigationObjectBuilder _navigationBuilder;

        public TableObjectBuilder(IColumnObjectBuilder columnBuilder, INavigationObjectBuilder navigationBuilder)
        {
            _columnBuilder = columnBuilder ?? throw new ArgumentNullException(nameof(columnBuilder));
            _navigationBuilder = navigationBuilder ?? throw new ArgumentNullException(nameof(navigationBuilder));
        }

        public TableObject BuildObjectFrom(IEntityType source)
        {
            var entity = source ?? throw new ArgumentNullException(nameof(source));

            var (tablePluralName, tableSingleName) = GetTableNames(entity);
            var assemblyFullName = GetAssemblyFullName(entity);
            var queryable = GetQueryableTableData(entity);
            var columnObjects = GetColumnObjects(entity).Concat(GetNavigationObjects(entity));

            return new TableObject(
                tablePluralName, assemblyFullName, entity.ClrType, queryable, columnObjects)
                .SetSingleName(tableSingleName);
        }

        private IEnumerable<ColumnObject> GetColumnObjects(IEntityType entityType)
        {
            return from property in entityType.GetProperties()
                   where !property.IsShadowProperty()
                   select _columnBuilder.BuildObjectFrom(property);
        }

        private IEnumerable<ColumnObject> GetNavigationObjects(IEntityType entityType)
        {
            return from navigation in entityType.GetNavigations()
                   where !navigation.IsShadowProperty()
                   select _navigationBuilder.BuildObjectFrom(navigation);
        }

        private (string TablePluralName, string TableSingleName) GetTableNames(IEntityType entityType)
        {
            var pluralName = GetTablePluralName(entityType);
            var singleName = GetTableSingleName(entityType);

            return (FormatTablePluralName(pluralName, singleName), singleName);
        }

        private static string FormatTablePluralName(string pluralName, string singleName)
            => pluralName == singleName ? $"{pluralName}s" : pluralName;

        private static string GetTablePluralName(IEntityType entityType)
            => entityType.IsOwned()
                    ? $"{entityType.GetTableName()}_{entityType.ClrType.Name}s"
                    : entityType.GetTableName();

        private static string GetTableSingleName(IEntityType entityType)
            => entityType.IsOwned()
                    ? $"{entityType.GetTableName()}_{entityType.ClrType.Name}"
                    : entityType.ClrType.Name;

        private TableData GetQueryableTableData(IEntityType entityType)
            => new TableData(
                          SetMethod.MakeGenericMethod(new Type[] { entityType.ClrType }),
                          entityType.GetNavigations().Select(navigation => navigation.Name));

        private string GetAssemblyFullName(IEntityType entityType)
            => entityType.ClrType.FullName;

        private static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));
    }
}
