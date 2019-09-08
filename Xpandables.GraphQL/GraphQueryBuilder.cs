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

using GraphQL.Types;

namespace System.GraphQL
{
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator

    /// <summary>
    /// <see cref="IGraphQueryBuilder"/> implementation.
    /// </summary>
    public sealed class GraphQueryBuilder : IGraphQueryBuilder
    {
        private readonly ITableObjectCollection _tableCollection;
        private readonly ITableObjectCollectionBuilder _tableCollectionBuilder;

        public GraphQueryBuilder(ITableObjectCollectionBuilder tableCollectionBuilder, ITableObjectCollection tableCollection)
        {
            _tableCollection = tableCollection ?? throw new ArgumentNullException(nameof(tableCollection));
            _tableCollectionBuilder = tableCollectionBuilder ?? throw new ArgumentNullException(nameof(tableCollectionBuilder));
        }

        public ObjectGraphType GetGraphQuery()
        {
            var graphQuery = new ObjectGraphType { Name = "Query", Description = "Generated query graph." };

            _tableCollectionBuilder.BuildTableObjectCollection();

            foreach (var tableObject in _tableCollection)
            {
                graphQuery.AddField(tableObject.Value.GetSingleFieldType());
                graphQuery.AddField(tableObject.Value.GetListFieldType());
            }

            return graphQuery;
        }
    }
}
