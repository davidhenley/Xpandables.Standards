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

using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Design.Database;
using System.GraphQL;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides methods to register GraphQL services
    /// </summary>
    public static class GraphQlServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the GraphQL implementation to the services.
        /// The target database should derived from <see cref="DataContext"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddCustomGraphQL(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ITableObjectCollection, TableObjectCollection>();
            services.AddScoped<ITableObjectCollectionBuilder, TableObjectCollectionBuilder>();

            services.AddScoped<ITableObjectBuilder, TableObjectBuilder>();
            services.AddScoped<IColumnObjectBuilder, ColumnObjectBuilder>();
            services.AddScoped<INavigationObjectBuilder, NavigationObjectBuilder>();

            services.AddScoped<IGraphQueryBuilder, GraphQueryBuilder>();

            services.AddScoped<ISchema, GraphQLSchema>();
            services.AddScoped<IDocumentExecuter, DocumentExecuter>();
            return services;
        }
    }
}
