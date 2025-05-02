using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RulesEnginePro.Core;
using RulesEnginePro.Models;

namespace RulesEngine.Api.Endpoints;

public static class WorkflowEndpoints
{
    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("workflows/identifiers", [Authorize] (
            [FromBody] WorkflowData workflow) =>
        {
            var identifiers = workflow.ExtractIdentifiers().ToList();
            return identifiers.GenerateSchema();
        })
        .WithName("ExtractWorkflowIdentifiers")
        .WithTags("Workflows")
        .Accepts<WorkflowData>("application/json")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Extract identifiers from workflow";
            op.Description = "Parses the given workflow and returns the list of identifiers found in the rule expressions.";
            return op;
        });

        app.MapPost("workflows/execute", [Authorize] async (
            [FromServices] IRulesEnginePro rulesEngine,
            [FromBody] InputWorkflow request,
            [FromQuery] string? ruleName,
            CancellationToken cancellationToken) =>
        {
            var result = await rulesEngine.ExecuteActionWorkflowAsync(request, ruleName, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ExecuteWorkflow")
        .WithTags("Workflows")
        .Accepts<InputWorkflow>("application/json")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Execute a workflow";
            op.Description = "Executes a workflow using Microsoft RulesEngine. Optionally filter by rule name.";
            return op;
        });

        app.MapPost("/workflows", [Authorize] async (
            HttpContext context,
            [FromServices] TimeProvider timeProvider,
            [FromServices] IWorkflowRepository repository,
            [FromBody] WorkflowData workflow,
            CancellationToken ct) =>
        {
            var user = context.GetUser();
            var audit = new Auditing(user, timeProvider.GetLocalNow());
            workflow.Auditing = audit;
            await repository.CreateWorkflowAsync(workflow, ct);

            return Results.Created($"/workflows/{workflow.WorkflowName}", workflow);
        })
        .WithName("CreateWorkflow")
        .WithTags("Workflows")
        .Accepts<WorkflowData>("application/json")
        .Produces<WorkflowData>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Create a new workflow";
            op.Description = "Creates and persists a new RulesEngine workflow definition.";
            return op;
        });

        app.MapGet("/workflows/{workflowName}", [Authorize] async (
            [FromRoute] string workflowName,
            [FromServices] IWorkflowRepository repository,
            CancellationToken ct) =>
        {
            var workflow = await repository.GetWorkflowAsync(workflowName, ct);
            return workflow is not null ? Results.Ok(workflow) : Results.NotFound();
        })
        .WithName("GetWorkflowByName")
        .WithTags("Workflows")
        .Produces<WorkflowData>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Get workflow by name";
            op.Description = "Retrieves a specific RulesEngine workflow by its name.";
            return op;
        });

        app.MapPut("/workflows/{workflowName}", [Authorize] async (
            HttpContext context,
            [FromRoute] string workflowName,
            [FromBody] WorkflowData workflow,
            [FromServices] TimeProvider timeProvider,
            [FromServices] IWorkflowRepository repository,
            CancellationToken ct) =>
        {
            if (workflowName != workflow.WorkflowName)
                return Results.BadRequest("WorkflowName in URL and body must match.");

            var user = context.GetUser();
            var audit = new Auditing(user, timeProvider.GetLocalNow());
            workflow.Auditing = audit;

            await repository.UpdateWorkflowAsync(workflow, ct);
            return Results.NoContent();
        })
        .WithName("UpdateWorkflow")
        .WithTags("Workflows")
        .Accepts<WorkflowData>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Update workflow";
            op.Description = "Updates an existing RulesEngine workflow. The name in the URL must match the one in the body.";
            return op;
        });

        app.MapDelete("/workflows/{workflowName}", [Authorize] async (
            [FromRoute] string workflowName,
            [FromServices] IWorkflowRepository repository,
            CancellationToken ct) =>
        {
            await repository.DeleteWorkflowAsync(workflowName, ct);
            return Results.NoContent();
        })
        .WithName("DeleteWorkflow")
        .WithTags("Workflows")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Delete workflow";
            op.Description = "Deletes a workflow definition by its name.";
            return op;
        });

        app.MapGet("/workflows", [Authorize] (
            HttpContext context,
            [FromQuery] string? workflowName,
            [FromServices] IWorkflowRepository repository,
            [AsParameters] PaginationQuery query,
            CancellationToken ct) =>
        {
            var workflows = repository.GetAllWorkflowsAsync(workflowName, query.Skip, query.Take, ct);
            return Results.Ok(workflows);
        })
        .WithName("ListWorkflows")
        .WithTags("Workflows")
        .Produces<IEnumerable<WorkflowData>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "List workflows";
            op.Description = "Retrieves a paginated list of workflows. You can filter by workflow name.";
            return op;
        });

        return app;
    }
}