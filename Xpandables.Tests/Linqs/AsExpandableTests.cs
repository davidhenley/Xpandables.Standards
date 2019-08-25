using System;
using System.Design.Linq;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Xpandables.Tests
{
    public class AsExpandableTests
    {
        [Fact]
        public void AsExpandable_With_Default_Optimizer()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            var first = PredicateBuilder.New<int>(i => i == 1);
            Expression<Func<int, bool>> second = i => i > 0;
            var predicate = first.Extend(second);

            // Act
            int result = query.AsExpandable().Where(predicate).First();

            // Assert
            Assert.Equal(1, result);
        }
    }
}