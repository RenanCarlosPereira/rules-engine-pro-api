using FluentAssertions;
using RulesEngine.Models;
using RulesEnginePro.Models;

namespace RulesEnginePro.Tests.Models;

public class WorkflowDataTests
{
    [Fact]
    public void ExtractIdentifiers_ShouldReturnNestedIdentifiers()
    {
        // Arrange
        var workflow = new WorkflowData
        {
            Rules = [new() { Expression = """person.name = "pedro" and x.age = 10 and y.address.city = "Sao Paulo" and company.name = "test" and h.invalid().ignored = true """ }],
        };

        // Act
        var result = workflow.ExtractIdentifiers();

        // Assert
        result.Should().BeEquivalentTo(
        "person.name",
        "x.age",
        "y.address.city",
        "company.name"
        // h.invalid().ignored → skipped due to method call
    );
    }

    [Fact]
    public void ExtractIdentifiers_ShouldReturnEmptyList_WhenNoRules()
    {
        // Arrange
        var workflow = new WorkflowData
        {
            Rules = null,
            GlobalParams = null
        };

        // Act
        var result = workflow.ExtractIdentifiers();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractIdentifiers_ShouldIgnoreMalformedExpressions()
    {
        // Arrange
        var workflow = new WorkflowData
        {
            Rules = [new Rule { Expression = "user.!" }]
        };

        // Act
        var result = workflow.ExtractIdentifiers();

        // Assert
        result.Should().BeEmpty(); // gracefully skip invalid expressions
    }

    [Fact]
    public void ExtractIdentifiers_ShouldAvoidSimpleIdentifiersWithoutDot()
    {
        // Arrange
        var workflow = new WorkflowData
        {
            Rules = [new Rule { Expression = "simpleIdentifier" }]
        };

        // Act
        var result = workflow.ExtractIdentifiers();

        // Assert
        result.Should().BeEmpty(); // only nested identifiers are included
    }
}