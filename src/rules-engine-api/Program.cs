using Microsoft.AspNetCore.HttpOverrides;
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Domain = ".rules-engine-pro.com";
});

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

// Forwarded Headers

var forwardedHeadersOptions = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto };
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// Cors config
app.UseCors("rules-engine-pro");

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapWorkflowEndpoints();
app.MapUserEndpoints();

await app.RunAsync();