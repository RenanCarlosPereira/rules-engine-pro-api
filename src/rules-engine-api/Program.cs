using Microsoft.AspNetCore.HttpOverrides;
using RulesEngine.Api.Endpoints;
using RulesEnginePro.Core;

string policy = "rules-engine-pro";

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRulesEnginePro();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
var frontendOrigin = builder.Configuration.GetSection("Cors:Urls").Get<string[]>() ?? [];

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(policy, policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseCors(policy);

app.UseSwagger();
app.UseSwaggerUI();

app.MapWorkflowEndpoints();
app.MapUserEndpoints();

await app.RunAsync();