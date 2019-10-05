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
using System.Design.Database;
using System.Linq;
using System.Reflection;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace System.GraphQL
{
#pragma warning disable HAA0302 // Display class allocation to capture closure
#pragma warning disable HAA0301 // Display class allocation to capture closure
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator

    /// <summary>
    /// Contains description table from the database.
    /// </summary>
    public class TableObject : ValueObject
    {
        public TableObject(
            string tableName, string assemblyFullName, Type tableType, TableData queryable, IEnumerable<ColumnObject> columns)
        {
            TableName = tableName;
            AssemblyFullName = assemblyFullName;
            TableType = tableType;
            Queryable = queryable;
            Columns = columns.ToList();
            SingleName = string.Empty;
            Description = string.Empty;
        }

        public string TableName { get; }
        public string AssemblyFullName { get; }
        public string SingleName { get; private set; }
        public Type TableType { get; }
        public string Description { get; private set; }
        public bool HasNavigationColumn => Columns.Any(column => column.IsNavigation);
        public TableData Queryable { get; }
        public ObjectGraphType<object>? TableObjectGraphType { get; private set; }
        public bool IsFieldTypeBuilt => TableObjectGraphType != null;
        public IList<ColumnObject> Columns { get; }

        public void BuildFieldType(ITableObjectCollection tableCollection)
        {
            if (IsFieldTypeBuilt)
                return;

            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].IsFieldTypeBuilt) continue;
                Columns[i].BuildFieldType(tableCollection);
            }

            if (Columns.All(a => a.IsFieldTypeBuilt))
            {
                if (TableObjectGraphType is null)
                    TableObjectGraphType = new ObjectGraphType<object> { Name = SingleName };

                foreach (var column in Columns)
                    TableObjectGraphType.AddField(column.FieldType);
            }
        }

        public FieldType GetListFieldType()
        {
            var listObjectGraphType = new ListGraphType(TableObjectGraphType);

            return new FieldType
            {
                Name = TableName.ToLowerInvariant(),
                Type = listObjectGraphType.GetType(),
                ResolvedType = listObjectGraphType,
                Resolver = new FieldResolverList(Queryable),
                Description = Description,
                Arguments = ProvideQueryArgumentsForList()
            };

            static QueryArguments ProvideQueryArgumentsForList()
              => new QueryArguments(
                  new QueryArgument<IntGraphType> { Name = "first", Description = "Total of elements", DefaultValue = 0 },
                  new QueryArgument<IntGraphType> { Name = "offset", Description = "Pagination", DefaultValue = 0 });
        }

        public FieldType GetSingleFieldType()
        {
            return new FieldType
            {
                Name = SingleName.ToLowerInvariant(),
                Type = TableObjectGraphType!.GetType(),
                ResolvedType = TableObjectGraphType,
                Resolver = new FieldResolverId(Queryable),
                Description = Description,
                Arguments = ProvideQueryArgumentsForId()
            };

            QueryArguments ProvideQueryArgumentsForId()
             => new QueryArguments(Columns
                  .Where(w => w.ColumnName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                  .Select(s => new QueryArgument<IdGraphType> { Name = s.ColumnName, Description = s.Description }));
        }

        public TableObject Describe(string description)
        {
            Description = description;
            return this;
        }

        public TableObject SetSingleName(string singleName)
        {
            SingleName = singleName;
            return this;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TableName;
            yield return TableType;
            yield return Queryable;
            yield return Description;
            yield return AssemblyFullName;
            foreach (var column in Columns.ToArray())
                yield return column;
        }
    }

    public class TableData
    {
        public TableData(MethodInfo source, IEnumerable<string> navigations)
        {
            Source = source;
            Navigations = navigations;
        }

        private MethodInfo Source { get; }
        private IEnumerable<string> Navigations { get; }

        public IQueryable<object> QueryOn(IDataContext dataContext)
        {
            var queryable = (IQueryable<object>)Source.Invoke(dataContext, null);
            foreach (var navigation in Navigations.ToArray())
                queryable = queryable.Include(navigation);

            return queryable;
        }
    }
}