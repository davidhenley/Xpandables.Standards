using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.Design.DependencyInjection;
using System.Linq;
using Xunit;

namespace Xpandables.TestsExtended
{
    public interface IDescriptor
    {
        int Id { get; }
    }

    public interface IContentDescriptor : IDescriptor { }

    public interface IUpdaterDescriptor : IDescriptor
    {
        void Update(object content);
    }

    public interface IUpdaterDescriptor<TContentDescriptor> : IUpdaterDescriptor
        where TContentDescriptor : IContentDescriptor
    {
        void Update(TContentDescriptor content);
    }

    public interface IUpdaterValidator
    {
        void Validate(object updater);
    }

    public interface IUpdaterValidator<TUpdaterDescriptor> : IUpdaterValidator
        where TUpdaterDescriptor : IUpdaterDescriptor
    {
        void Validate(TUpdaterDescriptor updater);
    }

    public interface IContentUpdater
    {
        void UpdateWith<TContentDescriptor>(IUpdaterDescriptor<TContentDescriptor> updater)
            where TContentDescriptor : class, IContentDescriptor;
    }

    public abstract class Descriptor : IDescriptor
    {
        public int Id { get; set; }
    }

    public abstract class ContentDescriptor : Descriptor, IContentDescriptor { }

    public abstract class UpdaterDescriptor<TContentDescriptor> : Descriptor, IUpdaterDescriptor<TContentDescriptor>
        where TContentDescriptor : IContentDescriptor
    {
        public abstract void Update(TContentDescriptor content);
        void IUpdaterDescriptor.Update(object content) => Update((TContentDescriptor)content);
    }

    public abstract class UpdaterValidator<TUpdaterDescriptor> : IUpdaterValidator<TUpdaterDescriptor>
        where TUpdaterDescriptor : IUpdaterDescriptor
    {
        public abstract void Validate(TUpdaterDescriptor updater);
        void IUpdaterValidator.Validate(object updater) => Validate((TUpdaterDescriptor)updater);
    }

    public class ContentVideoDescriptor : ContentDescriptor
    {
        public TimeSpan Duration { get; set; }
    }

    public class UpdaterVideoDescriptor : UpdaterDescriptor<ContentVideoDescriptor>
    {
        public TimeSpan Duration { get; set; }

        public override void Update(ContentVideoDescriptor content)
        {
            content.Duration = Duration;
        }
    }

    public class UpdateVideoValidator : UpdaterValidator<UpdaterVideoDescriptor>
    {
        public override void Validate(UpdaterVideoDescriptor updater)
        {
            if (updater.Duration > TimeSpan.FromMinutes(10))
                throw new ValidationException($"The specified duration is out of range");
        }
    }

    public class ContentOtherDescriptor : ContentDescriptor
    {
        public string Label { get; set; }
    }

    public class UpdaterOtherDescriptor : UpdaterDescriptor<ContentOtherDescriptor>
    {
        public string NewLabel { get; set; }

        public override void Update(ContentOtherDescriptor content)
        {
            content.Label = NewLabel;
        }
    }

    public class UpdaterOtherValidator : UpdaterValidator<UpdaterOtherDescriptor>
    {
        public override void Validate(UpdaterOtherDescriptor updater)
        {
            if (updater.NewLabel.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException($"The specified label should be less than {byte.MaxValue} characters.");
        }
    }

    public class ContentUpdateValidationDecorator : IContentUpdater
    {
        private readonly IContentUpdater _decoratee;
        private readonly IServiceProvider _provider;

        public ContentUpdateValidationDecorator(IContentUpdater decoratee, IServiceProvider provider)
        {
            _decoratee = decoratee ?? throw new ArgumentNullException(nameof(decoratee));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        void IContentUpdater.UpdateWith<TContent>(IUpdaterDescriptor<TContent> updater)
        {
            var validatorType = typeof(IUpdaterValidator<>).MakeGenericType(updater.GetType());
            var validator = _provider.XGetService<IUpdaterValidator>(validatorType).Cast<IUpdaterValidator>();
            validator?.Validate(updater);

            _decoratee.UpdateWith(updater);
        }
    }

    public class ContentUpdater : IContentUpdater
    {
        private readonly ContentContextDescriptor _dataContext;

        public ContentUpdater(ContentContextDescriptor dataContext)
            => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public void UpdateWith<TContent>(IUpdaterDescriptor<TContent> updater)
            where TContent : class, IContentDescriptor
        {
            ContentContextDescriptorSeeder.Seed(_dataContext); // just to seed database

            var content = _dataContext
                .Set<TContent>()
                .FirstOrDefault(c => c.Id == updater.Id)
                ?? throw new ValidationException($"Content with the '{updater.Id} does not exist.");

            updater.Update(content);
            _dataContext.SaveChanges();
        }
    }

    public class ContentTestsExtended
    {
        [Fact]
        public void SampleValid()
        {
            var services = new ServiceCollection()
                .Scan(scan => scan
                    .FromAssemblies(typeof(ContentTestsExtended).Assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IUpdaterValidator<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime())
                .AddDbContext<ContentContextDescriptor>(options =>
                    options.UseInMemoryDatabase(nameof(ContentContextDescriptor)), ServiceLifetime.Scoped)
                .AddTransient<IContentUpdater, ContentUpdater>();

            services.Decorate<IContentUpdater, ContentUpdateValidationDecorator>();
            var provider = services.BuildServiceProvider();

            var updateVideo = new UpdaterVideoDescriptor { Id = 1, Duration = TimeSpan.FromSeconds(10) };
            var updateOther = new UpdaterOtherDescriptor { Id = 1, NewLabel = "NewLabel" };

            var contentUpdater = provider.GetService<IContentUpdater>();
            contentUpdater.UpdateWith(updateVideo);
            contentUpdater.UpdateWith(updateOther);

            var context = provider.GetService<ContentContextDescriptor>();
            var contentVideo = context.ContentVideos.First(f => f.Id == updateVideo.Id);
            var contentOther = context.ContentOthers.First(f => f.Id == updateVideo.Id);

            Assert.Equal(updateVideo.Duration, contentVideo.Duration);
            Assert.Equal(updateOther.NewLabel, contentOther.Label);
        }

        [Fact]
        public void SampleNotValid()
        {
            var services = new ServiceCollection()
                .Scan(scan => scan
                    .FromAssemblies(typeof(ContentTestsExtended).Assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IUpdaterValidator<>))
                    .Where(_ => !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime())
                .AddDbContext<ContentContextDescriptor>(options =>
                    options.UseInMemoryDatabase(nameof(ContentContextDescriptor)), ServiceLifetime.Scoped)
                .AddTransient<IContentUpdater, ContentUpdater>();

            services.Decorate<IContentUpdater, ContentUpdateValidationDecorator>();
            var provider = services.BuildServiceProvider();

            var updateVideo = new UpdaterVideoDescriptor { Id = 1, Duration = TimeSpan.FromMinutes(50) };

            var contentUpdater = provider.GetService<IContentUpdater>();
            Assert.Throws<ValidationException>(() => contentUpdater.UpdateWith(updateVideo));
        }
    }


    public class ContentContextDescriptorSeeder
    {
        public static void Seed(ContentContextDescriptor contentContext)
        {
            if (contentContext.ContentVideos.Any() || contentContext.ContentOthers.Any())
                return;

            contentContext.Set<ContentVideoDescriptor>()
                .AddRange(new[]
                {
                    new ContentVideoDescriptor { Id = 1, Duration = TimeSpan.FromSeconds(120) },
                    new ContentVideoDescriptor { Id = 2, Duration = TimeSpan.FromSeconds(145) },
                    new ContentVideoDescriptor { Id = 3, Duration = TimeSpan.FromSeconds(155) }
                });

            contentContext.Set<ContentOtherDescriptor>()
                .AddRange(new[]
                {
                                new ContentOtherDescriptor { Id = 1, Label = "Current Label" },
                                new ContentOtherDescriptor { Id = 2, Label = "Content Other" }
                });

            contentContext.SaveChanges();
        }
    }


    public class ContentContextDescriptor : DbContext
    {
        public ContentContextDescriptor(DbContextOptions options) : base(options) { }

        public DbSet<ContentVideoDescriptor> ContentVideos { get; set; }
        public DbSet<ContentOtherDescriptor> ContentOthers { get; set; }
    }

}
