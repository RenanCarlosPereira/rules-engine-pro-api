using RulesEngine.Models;

namespace RulesEnginePro.Models;

public class WorkflowData : Workflow
{
    public Auditing? Auditing { get; set; }
}

public record Auditing(User User, DateTimeOffset LastModified);