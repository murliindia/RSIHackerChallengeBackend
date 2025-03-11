using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Moq;

namespace RSIHackerChallenge.Tests
{
    public class MiddlewareTests
    {
        [Test]
        public void ConfigureMiddleware_ShouldConfigureMiddlewaresCorrectly()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            ProgramExtensions.ConfigureServices(builder.Services, builder.Configuration);
            var app = builder.Build();

            // Act & Assert
            Assert.DoesNotThrow(() => ProgramExtensions.ConfigureMiddleware(app));
        }
    }
}
