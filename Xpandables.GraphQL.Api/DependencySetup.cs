using Microsoft.Extensions.DependencyInjection;
using System.Design.DependencyInjection;
using Xpandables.GraphQL.Api.Models;

namespace Xpandables.GraphQL.Api
{
    public static class DependencySetup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomGraphQL();
            services.AddCustomDataContext<GraphQLContextProvider>();
            services.AddCustomDataContextSeederDecorator<GraphQLContextSeeder>();
        }
    }
}