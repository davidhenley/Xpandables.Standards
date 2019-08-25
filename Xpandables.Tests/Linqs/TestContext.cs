using Microsoft.EntityFrameworkCore;

namespace Xpandables.Tests
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options) { }

        public DbSet<Entity> Entities { get; set; }
    }
}