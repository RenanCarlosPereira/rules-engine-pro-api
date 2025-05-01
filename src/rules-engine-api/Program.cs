using RulesEngine.Api;
using RulesEngine.Api.Endpoints;
using RulesEnginePro.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRulesEnginePro();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGithubAuthentication(builder.Configuration);

// Configure CORS
var frontendOrigin = builder.Configuration.GetSection("Cors:Urls").Get<string[]>() ?? [];
Console.WriteLine($"config cors are {string.Join(",", frontendOrigin)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("rules-engine-pro", policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseCors("rules-engine-pro");
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapWorkflowEndpoints();
app.MapUserEndpoints();

await app.RunAsync();