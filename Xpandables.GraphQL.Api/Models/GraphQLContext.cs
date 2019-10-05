using Microsoft.EntityFrameworkCore;
using System.Design.Database;

namespace Xpandables.GraphQL.Api.Models
{
    public class GraphQLContext : DataContext
    {
        public GraphQLContext(DbContextOptions<GraphQLContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Password> Passwords { get; set; }
    }
}