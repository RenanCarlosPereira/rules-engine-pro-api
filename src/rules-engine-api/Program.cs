using RulesEngine.Api;
using RulesEngine.Api.Endpoints;
using RulesEnginePro.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRulesEnginePro();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGithubAuthentication(builder.Configuration);

var frontendOrigin = builder.Configuration.GetSection("Cors:Urls").Get<string[]>() ?? [];

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
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("rules-engine-pro");
app.UseAuthentication();
app.UseAuthorization();
app.MapWorkflowEndpoints();
app.MapUserEndpoints();

await app.RunAsync();