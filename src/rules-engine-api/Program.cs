using RulesEngine.Api.Endpoints;
using RulesEnginePro.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRulesEnginePro();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapWorkflowEndpoints();

await app.RunAsync();