using Microsoft.Extensions.DependencyInjection;
using System.Design.DependencyInjection;

namespace Xpandables.GraphQL.Api
{
    public static class DependencySetup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomGraphQL();
            services.AddCustomDataContext<TestContextProvider>();
            services.AddCustomDataContextSeederDecorator<TestContextSeeder>();
        }
    }
}