using Microsoft.OpenApi.Models;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;
using RSIHackerChallenge.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace RSIHackerChallenge
{
    public static class ProgramExtensions
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Hacker News API",
                    Version = "v1",
                    Description = "It will show top stories",
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.Configure<ConfigurationSettings>(configuration.GetSection("ConfigurationSettings"));
            services.AddTransient<IStoryService, StoryService>();
            services.AddMemoryCache();
        }

        public static void ConfigureMiddleware(WebApplication app)
        {
            app.UseCors("CorsPolicy");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hacker News API v1");
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
