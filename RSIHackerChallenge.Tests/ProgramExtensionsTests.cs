using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;
using RSIHackerChallenge.Data;
using System.IO;

namespace RSIHackerChallenge.Tests
{
    public class ProgramExtensionsTests
    {
        private ServiceCollection _services;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        [Test]
        public void ConfigureServices_ShouldRegisterDependencies()
        {
            // Act
            ProgramExtensions.ConfigureServices(_services, _configuration);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            Assert.IsNotNull(serviceProvider.GetService<IStoryService>());
            Assert.IsInstanceOf<StoryService>(serviceProvider.GetService<IStoryService>());
             
        }
    }
}
