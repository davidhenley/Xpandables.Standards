using Microsoft.EntityFrameworkCore;
using System;
using System.Design.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Xpandables.Tests
{
    public class DbAsyncTests : Disposable
    {
        private TestContext _db;

        public DbAsyncTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase(nameof(DbAsyncTests));

            _db = new TestContext(builder.Options);

            _db.Entities.RemoveRange(_db.Entities.ToList());
            _db.Entities.AddRange(new[]
            {
                new Entity { Value = 123 },
                new Entity { Value = 67 },
                new Entity { Value = 3 }
            });
            _db.SaveChanges();
        }

        // Use TestCleanup to run code after each test has run
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _db = null;
            }

            base.Dispose(disposing);
        }

        [Fact]
        public async Task DbAsync_EnumerateShouldWorkAsync()
        {
            var task = _db.Entities.AsExpandable().ToListAsync();
            var result = await task.ConfigureAwait(false);
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task DbAsync_ExecuteShouldWorkAsync()
        {
            int expected = _db.Entities.Sum(e => e.Value);
            var task = _db.Entities.AsExpandable().SumAsync(e => e.Value);
            var result = await task.ConfigureAwait(false);
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task DbAsync_ExpressionInvokeTest()
        {
            var eParam = Expression.Parameter(typeof(Entity), "e");
            var eProp = Expression.PropertyOrField(eParam, "Value");

            var conditions =
                (from item in new List<int> { 10, 20, 30, 80 }
                 select Expression.LessThan(eProp, Expression.Constant(item))).Aggregate(Expression.OrElse);

            var combined = Expression.Lambda<Func<Entity, bool>>(conditions, eParam);

            var q = from e in _db.Entities.AsExpandable()
                    where combined.Invoke(e)
                    select new { e.Value };

            var res = await q.ToListAsync().ConfigureAwait(false);

            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);
        }

        [Fact]
        public async Task DbAsync_ExpressionStarter()
        {
            var combined = PredicateBuilder.New<Entity>();
            foreach (int i in new[] { 10, 20, 30, 80 })
            {
                var predicate = PredicateBuilder.New<Entity>(e => e.Value < i);
                combined = combined.Extend(predicate);
            }

            var q = _db.Entities.AsExpandable().Where(combined);
            var res = await q.ToListAsync().ConfigureAwait(false);

            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);
        }
    }
}