using Microsoft.Extensions.DependencyInjection;
using System;
using System.Design.Command;
using System.Design.DependencyInjection;
using Xunit;

namespace Xpandables.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void DecorateIsValid()
        {
            var services = new ServiceCollection()
                .AddTransient<IBaseClass<string>, BaseClass>()
                .Decorate(typeof(IBaseClass<>), typeof(BaseClassDecorator<>))
                .BuildServiceProvider();

            var instance = services.GetService<IBaseClass<string>>();

            instance.SetName("toto");
            Assert.Equal("Firsttoto", instance.Name);
        }

        [Fact]
        public void DecorateIsOpenValid()
        {
            var provider = new ServiceCollection()
                .AddTransient<IBaseClass<string>, BaseClass>()
                .XTryDecorate(typeof(IBaseClass<>), typeof(BaseClassDecorator<>))
                .BuildServiceProvider();

            var instance = provider.GetService<IBaseClass<string>>();

            instance.SetName("toto");
            Assert.Equal("Firsttoto", instance.Name);
        }
    }

    public interface IBaseClass<T>
        where T : class
    {
        T Name { get; }
        void SetName(T name);
    }
    public class BaseClass : IBaseClass<string>
    {
        public void SetName(string name) => Name = name;
        public string Name { get; private set; }
    }

    public class BaseClassDecorator<T> : IBaseClass<T>
        where T : class
    {
        private readonly IBaseClass<T> baseClass;
        public T Name => baseClass.Name;

        public BaseClassDecorator(IBaseClass<T> baseClass)
        {
            this.baseClass = baseClass ?? throw new ArgumentNullException(nameof(baseClass));
        }

        public void SetName(T name) => baseClass.SetName($"First{name}" as T);
    }
}
