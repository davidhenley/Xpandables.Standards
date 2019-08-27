using System;
using System.Collections.Generic;
using System.Design.Command;
using System.Text;
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
            var instance = creator.Create(type, (Action<CmdTest>)(cmd => { }));
            instance.Map(value => Assert.Equal(typeof(CommandHandlerBuilder<CmdTest>), value.GetType()));
        }

        class CmdTest : ICommand { }
    }
}
