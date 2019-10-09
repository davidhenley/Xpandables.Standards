using Microsoft.Extensions.Caching.Memory;
using System;
using System.Design.Command;
using System.Design.Database.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

        [Fact]
        public void ProviderFactoryInstance()
        {
            var provider = new DataProviderFactoryAccessor();
            var factory = provider.GetProviderFactory(DataProviderType.MSSQL);
            Assert.NotNull(factory);
        }

        [Fact]
        public void GetAllEnum()
        {
            var enums = EnumerationType.GetAll<EnumTwo>();
            Assert.Equal(2, enums.Count());
        }

        [Fact]
        public void GetValueEnum()
        {
            var value = EnumerationType.FromValue<EnumOne>(1);
            Assert.NotNull(value);
        }


        class CmdTest : ICommand { }

        public class EnumOne : EnumerationType
        {
            protected EnumOne(string displayName, int value) : base(displayName, value) { }
            public static EnumOne One => new EnumOne("One", 1);
        }

        public class EnumTwo : EnumOne
        {
            protected EnumTwo(string displayName, int value) : base(displayName, value) { }
            public static EnumTwo Two => new EnumTwo("Two", 1);
        }
    }
}
