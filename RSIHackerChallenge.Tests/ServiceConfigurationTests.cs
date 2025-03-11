using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using RSIHackerChallenge;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Primitives;
using RSIHackerChallenge.Data;

namespace RSIHackerChallenge.Tests
{
    public class ServiceConfigurationTests
    {
        private ServiceCollection _services;
        private Mock<IConfiguration> _configurationMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            _configurationMock = new Mock<IConfiguration>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
        }

        [Test]
        public void ConfigureServices_ShouldRegisterServicesCorrectly()
        {
            // Arrange: Setup mock for IConfiguration
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConfigurationSettings:SomeSetting", "SomeValue" }
            };

            // Create the mock configuration section with "ConfigurationSettings:SomeSetting"
            var mockSection = new ConfigurationSectionMock(inMemorySettings, "ConfigurationSettings", "ConfigurationSettings");

            _configurationMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(mockSection);

            // Mock ILoggerFactory and add it to the services collection
            _services.AddSingleton<ILoggerFactory>(_loggerFactoryMock.Object);

            // Act: Call ConfigureServices to add services to the container
            ProgramExtensions.ConfigureServices(_services, _configurationMock.Object);

            // Assert: Verify that services have been correctly registered in the DI container

            // Assert that controllers have been added by checking IControllerFactory
            var controllerFactory = _services.BuildServiceProvider().GetService<IControllerFactory>();
            Assert.IsNotNull(controllerFactory, "Controllers have not been registered correctly.");

            // Assert that CORS policy has been added
            var corsOptions = _services.BuildServiceProvider().GetService<IOptions<CorsOptions>>();
            Assert.IsNotNull(corsOptions, "CORS options have not been registered correctly.");

            // Check if the CORS policy "CorsPolicy" exists
            var corsPolicy = corsOptions.Value.GetPolicy("CorsPolicy");
            Assert.IsNotNull(corsPolicy, "CORS policy 'CorsPolicy' has not been registered.");

            // Assert that ConfigurationSettings are configured
            var configurationSettings = _services.BuildServiceProvider().GetService<IOptions<ConfigurationSettings>>();
            Assert.IsNotNull(configurationSettings, "ConfigurationSettings have not been registered correctly.");

            // Assert that IStoryService has been added
            var storyService = _services.BuildServiceProvider().GetService<IStoryService>();
            Assert.IsNotNull(storyService, "IStoryService has not been registered correctly.");

            // Assert that memory cache has been added
            var memoryCacheService = _services.BuildServiceProvider().GetService<IMemoryCache>();
            Assert.IsNotNull(memoryCacheService, "MemoryCache has not been registered correctly.");
        }
    }

    // Mock class for IConfigurationSection to return in-memory settings
    public class ConfigurationSectionMock : IConfigurationSection
    {
        private readonly IDictionary<string, string> _settings;
        public string Key { get; set; }
        public string Path { get; set; }

        public ConfigurationSectionMock(IDictionary<string, string> settings, string key = "", string path = "")
        {
            _settings = settings;
            Key = key;
            Path = path;
        }

        public string Value
        {
            get => _settings.ContainsKey(Key) ? _settings[Key] : null;
            set => throw new System.NotImplementedException();
        }

        public IEnumerable<IConfigurationSection> GetChildren() => new List<IConfigurationSection>(); // Mock implementation

        public IChangeToken GetReloadToken() => throw new System.NotImplementedException();

        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationSectionMock(_settings, key, $"{Path}:{key}");
        }

        public string this[string key]
        {
            get => _settings.ContainsKey(key) ? _settings[key] : null;
            set => throw new System.NotImplementedException();
        }
    }
}
