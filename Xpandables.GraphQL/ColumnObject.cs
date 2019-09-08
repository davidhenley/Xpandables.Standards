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
using GraphQL.Types;

namespace System.GraphQL
{
    /// <summary>
    /// Contains a description of a column in a table object.
    /// </summary>
    public class ColumnObject : ValueObject
    {
        public ColumnObject(string columnName, Type dataType, string description)
        {
            ColumnName = columnName;
            DataType = dataType;
            Description = description;
        }

        public string ColumnName { get; }
        public Type DataType { get; }
        public string Description { get; }
        public FieldType? FieldType { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsNavigation { get; private set; }
        public bool IsEnumerable { get; private set; }
        public bool IsFieldTypeBuilt => FieldType != null;

        public ColumnObject Nullable(bool isNullable)
        {
            IsNullable = isNullable;
            return this;
        }

        public ColumnObject Enumerable(bool isEnumerable)
        {
            IsEnumerable = isEnumerable;
            return this;
        }

        public ColumnObject Navigation(bool isNavigation)
        {
            IsNavigation = isNavigation;
            return this;
        }

        public void BuildFieldType(ITableObjectCollection tableCollection)
        {
            if (IsFieldTypeBuilt)
                return;

            Type fieldType;
            dynamic fieldInstance = new ObjectGraphType<object>();

            if (IsNavigation)
            {
                if (GetTableFromCollection(tableCollection, DataType) is TableObject tableObject && tableObject.IsFieldTypeBuilt)
                {
                    if (IsEnumerable)
                        fieldInstance = new ListGraphType(tableObject.TableObjectGraphType);
                    else
                        fieldInstance = tableObject.TableObjectGraphType!;

                    fieldType = fieldInstance!.GetType();
                }
                else
                {
                    // Waiting for a new pass
                    return;
                }
            }
            else
            {
                fieldType = DataType.GetGraphTypeFromTypeUsingConverter(IsNullable);
            }

            FieldType = new FieldType
            {
                Type = fieldType,
                Name = ColumnName,
                Description = Description,
                Resolver = new NameFieldResolver(),
                ResolvedType = IsNavigation ? fieldInstance : default
                //Arguments = column.IsEnumerable ? ProvideQueryArgumentsForList() : default TODO ?
            };
        }

        private static TableObject GetTableFromCollection(ITableObjectCollection collection, Type target)
            => (from table in collection
                where table.Value.TableType == target
                select table.Value)
                    .FirstOrDefault();

//#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ColumnName;
            yield return DataType;
            yield return Description;
            yield return IsNullable;
            yield return IsNavigation;
            yield return IsEnumerable;
            yield return IsFieldTypeBuilt;
        }
    }
}