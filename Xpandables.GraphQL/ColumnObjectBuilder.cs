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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace System.GraphQL
{
    public sealed class ColumnObjectBuilder : IColumnObjectBuilder
    {
        public ColumnObject BuildObjectFrom(IProperty source)
        {
            var property = source ?? throw new ArgumentNullException(nameof(source));

            return new ColumnObject(
                    property.DeclaringEntityType.IsOwned() ? property.Name : property.Relational().ColumnName,
                    property.ClrType,
                    property.PropertyInfo.GetDescription())
                .Nullable(property.IsNullable);
        }
    }
}
