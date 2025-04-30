using RulesEngine.Models;
using RulesEnginePro.Models;

namespace RulesEnginePro.Core;

public interface IRulesEnginePro
{
    Task<ActionRuleResult> ExecuteActionWorkflowAsync(InputWorkflow request, string? ruleName = default, CancellationToken cancellationToken = default);
}
