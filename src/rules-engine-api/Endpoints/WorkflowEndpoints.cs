using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RulesEngine.Models;
using RulesEnginePro.Core;
using RulesEnginePro.Models;

namespace RulesEngine.Api.Endpoints;

public static class WorkflowEndpoints
{
    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("workflows/identifiers", [Authorize] (Workflow workflow) =>
        {
            var identifies = workflow.ExtractIdentifiers().ToList();
            return identifies.GenerateSchema();
        });

        app.MapPost("workflows/execute", [Authorize] async ([FromServices] IRulesEnginePro rulesEngine, [FromBody] InputWorkflow request, [FromQuery] string? ruleName, CancellationToken cancellationToken) =>
        {
            var result = await rulesEngine.ExecuteActionWorkflowAsync(request, ruleName, cancellationToken);
            return Results.Ok(result);
        });

        app.MapPost("/workflows", [Authorize] async (Workflow workflow, [FromServices] IWorkflowRepository repository, CancellationToken ct) =>
        {
            await repository.CreateWorkflowAsync(workflow, ct);
            return Results.Created($"/workflows/{workflow.WorkflowName}", workflow);
        });

        app.MapGet("/workflows/{workflowName}", [Authorize] async (string workflowName, [FromServices] IWorkflowRepository repository, CancellationToken ct) =>
        {
            var workflow = await repository.GetWorkflowAsync(workflowName, ct);
            return workflow is not null ? Results.Ok(workflow) : Results.NotFound();
        });

        app.MapPut("/workflows/{workflowName}", [Authorize] async (string workflowName, Workflow workflow, [FromServices] IWorkflowRepository repository, CancellationToken ct) =>
        {
            if (workflowName != workflow.WorkflowName)
                return Results.BadRequest("WorkflowName in URL and body must match.");

            await repository.UpdateWorkflowAsync(workflow, ct);
            return Results.NoContent();
        });

        app.MapDelete("/workflows/{workflowName}", [Authorize] async (string workflowName, [FromServices] IWorkflowRepository repository, CancellationToken ct) =>
        {
            await repository.DeleteWorkflowAsync(workflowName, ct);
            return Results.NoContent();
        });

        app.MapGet("/workflows", [Authorize] async (HttpContext context, [FromQuery] string? workflowName, [FromServices] IWorkflowRepository repository, [AsParameters] PaginationQuery query, CancellationToken ct) =>
        {
            var workflows = repository.GetAllWorkflowsAsync(workflowName, query.Skip, query.Take, ct);
            return Results.Ok(workflows);
        });

        return app;
    }
}