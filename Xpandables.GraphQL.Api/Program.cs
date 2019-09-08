using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Xpandables.GraphQL.Api
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    // Adding dependency injection configuration
                })
               .UseStartup<Startup>()
               .Build();
    }
}