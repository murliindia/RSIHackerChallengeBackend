using Microsoft.OpenApi.Models;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;
using RSIHackerChallenge.Data;
using System.Diagnostics.CodeAnalysis;

#if !DEBUG
[ExcludeFromCodeCoverage]
#endif
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hacker News API",
        Version = "v1",
        Description = "It will shows top stories",
    });

});


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

IConfiguration configuration = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json")
       .Build();

builder.Services.Configure<ConfigurationSettings>(configuration.GetSection("ConfigurationSettings"));


builder.Services.AddTransient<IStoryService, StoryService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// In Configure method
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
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

app.Run();
