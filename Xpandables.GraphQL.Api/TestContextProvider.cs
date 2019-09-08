using System;
using System.Design.Database;
using Microsoft.EntityFrameworkCore;

namespace Xpandables.GraphQL.Api
{
    public class TestContextProvider : IDataContextProvider
    {
        public Optional<IDataContext> GetDataContext()
        {
            return new DbContextOptionsBuilder<TestContext>()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseInMemoryDatabase(nameof(TestContext))
                .ToOptional()
                .Map(options => new TestContext(options.Options))
                .CastOptional<IDataContext>();
        }
    }
}