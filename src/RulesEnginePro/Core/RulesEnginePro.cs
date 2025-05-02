using RulesEngine.HelperFunctions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using RulesEnginePro.Models;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace RulesEnginePro.Core;

internal class RulesEnginePro(IRulesEngine rulesEngine) : IRulesEnginePro
{
    public async Task<ActionRuleResult> ExecuteActionWorkflowAsync(InputWorkflow request, string? ruleName = default, CancellationToken cancellationToken = default)
    {
        var workflow = request.WorkflowData;
        var inputs = request.Inputs;

        var dictionary = inputs.Deserialize<Dictionary<string, JsonElement?>>();
        var parameters = dictionary?.Select(x => RuleParameter.Create(x.Key, x.Value?.ToExpandoObject())).OfType<RuleParameter>().ToArray() ?? [];

        rulesEngine.AddOrUpdateWorkflow(workflow);

        if (!string.IsNullOrWhiteSpace(ruleName))
            return await rulesEngine.ExecuteActionWorkflowAsync(workflow.WorkflowName, ruleName, parameters, cancellationToken);

        var ruleResultTree = await rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName, parameters, cancellationToken);
        return new ActionRuleResult()
        {
            Results = ruleResultTree,
            Output = null,
        };
    }
}