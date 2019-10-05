using System.Design.Database;
using System.Linq;

namespace Xpandables.GraphQL.Api.Models
{
    public class GraphQLContextSeeder : IDataContextSeeder
    {
        public IDataContext Seed(IDataContext dataContext)
        {
            if (dataContext.SetOf<Person>().Count() > 0)
                return dataContext;

            var person1 = new Person
            {
                FirstName = "Florian",
                LastName = "SERGE",
                Address = new Address
                {
                    City = "Paris",
                    Street = "Tête d'or"
                },
                Password = new Password { Value = "motdepasse" },
                Children = new[]
                        {
                            new Child
                            {
                                Name = "Hawai",
                                Age = 16
                            },
                            new Child
                            {
                                Name = "Linda",
                                Age = 11
                            }
                        }
            };

            var person2 = new Person
            {
                FirstName = "Sandrine",
                LastName = "LEVASSEUR",
                Address = new Address
                {
                    City = "London",
                    Street = "Street"
                },
                Password = new Password { Value = "motdepasse1" },
                Children = new[]
                        {
                            new Child
                            {
                                Name = "Arthur",
                                Age = 17
                            },
                            new Child
                            {
                                Name = "Jean",
                                Age = 15
                            }
                        }
            };

            dataContext.AddRange(new[] { person1, person2 });
            return dataContext;
        }
    }
}