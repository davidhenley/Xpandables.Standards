using System;
using Xunit;

namespace Xpandables.Tests
{
    public class EnumTest
    {
        [Fact]
        public void CreateInstance()
        {
            Test value = new Test1<int>();
            if(value is int v)
            {

            }
        }
    }

    public class Test { }
    public class Test1<T> : Test { }

    public class EnumFirst : EnumerationType
    {
        protected EnumFirst(string displayName, int value) : base(displayName, value) { }
        public static EnumFirst First => new EnumFirst("First", 1);
        public static EnumFirst Second => new EnumFirst("Second", 2);
    }

    public class EnumSecond : EnumFirst
    {
        protected EnumSecond(string displayName, int value) : base(displayName, value) { }
        public static EnumSecond Third => new EnumSecond("Third", 3);
        public static EnumSecond Quatro => new EnumSecond("Quatro", 4);
    }
}
