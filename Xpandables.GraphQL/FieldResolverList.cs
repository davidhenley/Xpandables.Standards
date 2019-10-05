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
using System.Linq.Dynamic.Core;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace System.GraphQL
{
    /// <summary>
    /// Resolves field from context.
    /// </summary>
    public sealed class FieldResolverList : IFieldResolver
    {
        private readonly TableData tableData;

        public FieldResolverList(TableData tableData)
            => this.tableData = tableData ?? throw new ArgumentNullException(nameof(tableData));

        public object Resolve(ResolveFieldContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var first = context.GetArgument("first", int.MaxValue);
            var offset = context.GetArgument("offset", 0);
            var dataSource = ((GraphQLSchema)context.Schema).DependencyResolver.Resolve<IDataContext>();

            return tableData.QueryOn(dataSource)
                    .Skip(offset)
                    .Take(first)
                    .ToDynamicList<object>();
        }
    }
}