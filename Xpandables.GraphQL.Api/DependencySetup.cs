using Microsoft.Extensions.DependencyInjection;
using System.Design.DependencyInjection;
using Xpandables.GraphQL.Api.Models;

namespace Xpandables.GraphQL.Api
{
    public static class DependencySetup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddXGraphQL();
            services.AddXDataContext<GraphQLContextProvider>();
            services.AddXDataContextSeederBehavior<GraphQLContextSeeder>();
        }
    }
}