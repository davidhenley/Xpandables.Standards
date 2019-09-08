using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xpandables.GraphQL.Api.Models
{
    [Table(nameof(Person))]
    public class Person : Entity
    {
        [System.ComponentModel.Description("Person first name.")]
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public Address Address { get; set; }
        public Password Password { get; set; }
        public ICollection<Child> Children { get; set; }
    }
}