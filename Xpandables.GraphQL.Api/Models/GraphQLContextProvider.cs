using System;
using System.Design.Database;
using Microsoft.EntityFrameworkCore;

namespace Xpandables.GraphQL.Api.Models
{
    public class GraphQLContextProvider : IDataContextProvider
    {
        public Optional<IDataContext> GetDataContext()
        {
            return new DbContextOptionsBuilder<GraphQLContext>()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseInMemoryDatabase(nameof(GraphQLContext))
                .AsOptional()
                .Map(options => new GraphQLContext(options.Options))
                .CastOptional<IDataContext>();
        }
    }
}