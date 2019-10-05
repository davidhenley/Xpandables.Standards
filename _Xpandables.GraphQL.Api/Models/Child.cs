using System;

namespace Xpandables.GraphQL.Api.Models
{
    public class Child : Entity
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}