using RulesEngine.Models;
using System.Text.Json;
using RulesEngine.HelperFunctions;
using Microsoft.AspNetCore.Mvc;
using RulesEngine.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

app.UseCors();

app.MapPost("/identifiers", (Workflow workflow) =>
{
    var identifies = workflow.ExtractIdentifiers().ToList();
    return identifies.GenerateSchema();
})
.WithName("GetIdentifiers");


app.MapPost("/execute", async ([FromBody] InputWorkflow request, [FromQuery] string? ruleName) =>
{
    var workflow = request.Workflow;
    var inputs = request.Inputs;
    var dictionary = inputs.Deserialize<Dictionary<string, JsonElement?>>();
    var parameters = dictionary?.Select(x => RuleParameter.Create(x.Key, x.Value?.ToExpandoObject())).OfType<RuleParameter>().ToArray() ?? [];

    var reSettings = new ReSettings { CustomTypes = [typeof(RulesEngine.Api.Utils)] };
    var rulesEngine = new RulesEngine.RulesEngine([workflow], reSettings);

    if (!string.IsNullOrWhiteSpace(ruleName))
        return Results.Ok(await rulesEngine.ExecuteActionWorkflowAsync(workflow.WorkflowName, ruleName, parameters));

    return Results.Ok(await rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName, parameters));

}).WithName("ExecuteWorkFlow");

app.Run();

public record InputWorkflow(JsonElement Inputs, Workflow Workflow);


