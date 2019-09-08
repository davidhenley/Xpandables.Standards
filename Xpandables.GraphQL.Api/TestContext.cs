using Microsoft.EntityFrameworkCore;
using System;
using System.Design.Database;
using Xpandables.GraphQL.Api.Models;

namespace Xpandables.GraphQL.Api
{
    public class TestContext : DataContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Password> Passwords { get; set; }
    }
}