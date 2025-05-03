using RulesEnginePro.Actions;

namespace RulesEngine.Api.Endpoints;

public static class ActionEndpoints
{
    public static IEndpointRouteBuilder MapActionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/action/context-definitions",
             (IContextActionService service, CancellationToken cancellationToken) =>
            {
                var result = service.GetContextActions(cancellationToken).ToBlockingEnumerable(cancellationToken: cancellationToken);
                return TypedResults.Ok(result);
            })
            .WithName("GetContextDefinitions")
            .WithTags("Actions")
            .WithOpenApi(op =>
            {
                op.Summary = "Get all context action definitions";
                op.Description = "Returns a dictionary of all registered context actions and their field definitions.";
                return op;
            });

        return app;
    }
}