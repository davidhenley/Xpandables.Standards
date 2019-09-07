using Microsoft.Extensions.Caching.Memory;
using System;
using System.Design.Command;
using System.Diagnostics;
using Xunit;

namespace Xpandables.Tests
{
    public class InstanceCreatorTests
    {
        [Fact]
        public void CreateInstance()
        {
            var type = typeof(CommandHandlerBuilder<>).MakeGenericType(typeof(CmdTest));
            var creator = new InstanceCreator();
            //instance.Map(value => Assert.Equal(typeof(CommandHandlerBuilder<CmdTest>), value.GetType()));
            var watch = new Stopwatch();

            for (int i = 0; i < 100; i++)
            {
                watch.Start();
                var instance = creator.Create(type, (Action<CmdTest>)(cmd => { }));
                watch.Stop();
                Debug.WriteLine($"Elapsed time : {watch.Elapsed}");
            }
        }

        [Fact]
        public void CreateInstanceCache()
        {
            var creator = new InstanceCreatorCache(new MemoryCache(new MemoryCacheOptions()));
            var type = typeof(CommandHandlerBuilder<>).MakeGenericType(typeof(CmdTest));
            var watch = new Stopwatch();

            for (int i = 0; i < 100; i++)
            {
                watch.Start();
                var instance = creator.Create(type, (Action<CmdTest>)(cmd => { }));
                watch.Stop();
                Debug.WriteLine($"Elapsed time : {watch.Elapsed}");
            }

            //instance.Map(value => Assert.Equal(typeof(CommandHandlerBuilder<CmdTest>), value.GetType()));
        }


        class CmdTest : ICommand { }
    }
}
