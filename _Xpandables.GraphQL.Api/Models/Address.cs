using System;

namespace Xpandables.GraphQL.Api.Models
{
    public class Address : Entity
    {
        public string Street { get; set; }
        public string City { get; set; }
    }
}