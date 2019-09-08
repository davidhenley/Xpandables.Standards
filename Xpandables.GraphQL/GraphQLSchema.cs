﻿/************************************************************************************************************
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
    /// <summary>
    /// The base graphQL schema.
    /// TODO : add mutation.
    /// </summary>
    public sealed class GraphQLSchema : Schema
    {
        public GraphQLSchema(IGraphQueryBuilder graphQueryBuilder, IServiceProvider serviceProvider)
        {
            Query = graphQueryBuilder.GetGraphQuery();
            DependencyResolver = new DependencyResolver(serviceProvider);
        }
    }
}