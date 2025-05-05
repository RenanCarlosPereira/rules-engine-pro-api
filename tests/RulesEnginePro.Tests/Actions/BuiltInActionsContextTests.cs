using FluentAssertions;
using RulesEnginePro.Actions;
using static RulesEnginePro.Actions.ContextActionBase;

namespace RulesEnginePro.Tests.Actions;

public class BuiltInActionsContextTests
{
    [Fact]
    public async Task GetBuiltInDefinitions_ShouldReturnExpectedActions()
    {
        // Act
        var result = new List<KeyValuePair<string, Dictionary<string, FieldDefinition>>>();
        await foreach (var item in BuiltInActionsContext.GetBuiltInDefinitions())
        {
            result.Add(item);
        }

        // Assert
        result.Should().HaveCount(2);

        var outputExpression = result.SingleOrDefault(x => x.Key == "OutputExpression");
        var evaluateRule = result.SingleOrDefault(x => x.Key == "EvaluateRule");

        outputExpression.Value.Should().ContainKey("Expression");
        outputExpression.Value["Expression"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "lambda espression",
            Required = true
        });

        evaluateRule.Value.Should().ContainKeys("workflowName", "ruleName");
        evaluateRule.Value["workflowName"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "workflowName",
            Required = true
        });
        evaluateRule.Value["ruleName"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "ruleName",
            Required = true
        });
    }
}