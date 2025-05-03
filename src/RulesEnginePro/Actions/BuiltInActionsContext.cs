using static RulesEnginePro.Actions.ContextActionBase;

namespace RulesEnginePro.Actions;

public static class BuiltInActionsContext
{
    public static async IAsyncEnumerable<KeyValuePair<string, Dictionary<string, FieldDefinition>>> GetBuiltInDefinitions()
    {
        await Task.Yield();
        var outputExpression = new Dictionary<string, FieldDefinition>
        {
            ["Expression"] = new FieldDefinition
            {
                Type = "string",
                Label = "lambda espression",
                Required = true
            },
        };

        var evaluateRule = new Dictionary<string, FieldDefinition>
        {
            ["workflowName"] = new FieldDefinition
            {
                Type = "string",
                Label = "workflowName",
                Required = true
            },
            ["ruleName"] = new FieldDefinition
            {
                Type = "string",
                Label = "ruleName",
                Required = true
            },
        };

        yield return new KeyValuePair<string, Dictionary<string, FieldDefinition>>("OutputExpression", outputExpression);
        yield return new KeyValuePair<string, Dictionary<string, FieldDefinition>>("EvaluateRule", evaluateRule);
    }
}