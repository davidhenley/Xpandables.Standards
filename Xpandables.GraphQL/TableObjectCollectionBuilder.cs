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

using System.Design.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace System.GraphQL
{
    public sealed class TableObjectCollectionBuilder : ITableObjectCollectionBuilder
    {
        private readonly DbContext _dbContext;
        private readonly ITableObjectBuilder _tableBuilder;
        private readonly ITableObjectCollection _tableCollection;

        public TableObjectCollectionBuilder(
            IDataContext dataContext, ITableObjectBuilder tableBuilder, ITableObjectCollection tableCollection)
        {
            _dbContext = dataContext as DbContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tableBuilder = tableBuilder ?? throw new ArgumentNullException(nameof(tableBuilder));
            _tableCollection = tableCollection ?? throw new ArgumentNullException(nameof(tableCollection));
        }

        public void BuildTableObjectCollection()
        {
            foreach (var entityType in _dbContext.Model.GetEntityTypes())
            {
                var tableObject = _tableBuilder.BuildObjectFrom(entityType);
                _tableCollection[tableObject.AssemblyFullName] = tableObject;
            }

            while (_tableCollection.Select(kv => kv.Value).Any(t => !t.IsFieldTypeBuilt))
            {
                foreach (var tableObject in _tableCollection)
                {
                    if (tableObject.Value.IsFieldTypeBuilt) continue;
                    tableObject.Value.BuildFieldType(_tableCollection);
                }
            }
        }
    }
}
