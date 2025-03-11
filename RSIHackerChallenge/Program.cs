using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RSIHackerChallenge;

var builder = WebApplication.CreateBuilder(args);

ProgramExtensions.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
ProgramExtensions.ConfigureMiddleware(app);

app.Run();
