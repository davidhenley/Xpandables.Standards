using System;
using System.Design.Database;
using System.GraphQL;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Xpandables.GraphQL.Api
{
    public class DependencySetup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITableObjectCollection, TableObjectCollection>();
            services.AddScoped<ITableObjectCollectionBuilder, TableObjectCollectionBuilder>();

            services.AddScoped<ITableObjectBuilder, TableObjectBuilder>();
            services.AddScoped<IColumnObjectBuilder, ColumnObjectBuilder>();
            services.AddScoped<INavigationObjectBuilder, NavigationObjectBuilder>();

            services.AddScoped<IGraphQueryBuilder, GraphQueryBuilder>();

            services.AddScoped<ISchema, GraphQLSchema>();
            services.AddScoped<IDocumentExecuter, DocumentExecuter>();

            services.AddScoped<IDataContextProvider, TestContextProvider>();
            services.AddScoped<IDataContextSeeder, TestContextSeeder>();
            services.Decorate<IDataContextProvider, DataContextProviderSeederDecorator>();

            services.AddScoped(serviceProvider =>
            {
                var dataContextProvider = serviceProvider.GetRequiredService<IDataContextProvider>();
                return dataContextProvider.GetDataContext().Return();
            });

            services.AddScoped<IDataContextSeeder, TestContextSeeder>();
        }
    }
}