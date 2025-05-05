using Bogus;
using FluentAssertions;
using Moq;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using RulesEnginePro.Models;
using System.Text.Json;

namespace RulesEnginePro.Tests.Core;

public class RulesEngineProTests
{
    private readonly Mock<IRulesEngine> _mockRulesEngine;
    private readonly Faker _faker;
    private readonly RulesEnginePro.Core.RulesEnginePro _sut;

    public RulesEngineProTests()
    {
        _mockRulesEngine = new Mock<IRulesEngine>();
        _sut = new RulesEnginePro.Core.RulesEnginePro(_mockRulesEngine.Object);
        _faker = new Faker();
    }

    [Fact]
    public async Task ExecuteActionWorkflowAsync_WithRuleName_ShouldCallExecuteActionWorkflowAsync()
    {
        // Arrange
        var workflowName = _faker.Random.Word();
        var ruleName = _faker.Random.Word();
        var workflow = new WorkflowData { WorkflowName = workflowName };

        var inputs = new Dictionary<string, object?>
    {
        { "input1", new { value = 1 } },
        { "input2", new { value = "test" } }
    };

        var jsonInputs = JsonSerializer.Serialize(inputs);
        var request = new InputWorkflow(JsonDocument.Parse(jsonInputs).RootElement, workflow);

        _mockRulesEngine
            .Setup(x => x.ExecuteActionWorkflowAsync(workflowName, ruleName, It.IsAny<RuleParameter[]>(), It.IsAny<CancellationToken>()))
            .Returns(() => ValueTask.FromResult(new ActionRuleResult()));

        // Act
        _ = await _sut.ExecuteActionWorkflowAsync(request, ruleName);

        // Assert
        _mockRulesEngine.Verify(x => x.ExecuteActionWorkflowAsync(workflowName, ruleName, It.IsAny<RuleParameter[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteActionWorkflowAsync_WithoutRuleName_ShouldCallExecuteAllRulesAsync()
    {
        // Arrange
        var workflowName = _faker.Random.Word();
        var workflow = new WorkflowData { WorkflowName = workflowName };

        var inputs = new Dictionary<string, object?>
    {
        { "item1", new { data = 123 } }
    };

        var jsonInputs = JsonSerializer.Serialize(inputs);
        var request = new InputWorkflow(JsonDocument.Parse(jsonInputs).RootElement, workflow);

        var expectedResults = new List<RuleResultTree>
    {
        new() { Rule = new Rule{ RuleName = "rule1" }  },
        new() { Rule = new Rule{ RuleName = "rule1" }  }
    };

        _mockRulesEngine
            .Setup(x => x.ExecuteAllRulesAsync(workflowName, It.IsAny<RuleParameter[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var result = await _sut.ExecuteActionWorkflowAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
        result.Output.Should().BeNull();
        _mockRulesEngine.Verify(x => x.AddOrUpdateWorkflow(workflow), Times.Once);
        _mockRulesEngine.Verify(x => x.ExecuteAllRulesAsync(workflowName, It.IsAny<RuleParameter[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteActionWorkflowAsync_WithEmptyInputs_ShouldNotFail()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = _faker.Random.Word() };
        var jsonInputs = JsonSerializer.Serialize(JsonDocument.Parse("{}").RootElement);
        var request = new InputWorkflow(JsonDocument.Parse(jsonInputs).RootElement, workflow);

        _mockRulesEngine
            .Setup(x => x.ExecuteAllRulesAsync(It.IsAny<string>(), It.IsAny<RuleParameter[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.ExecuteActionWorkflowAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().BeEmpty();
        _mockRulesEngine.Verify(x => x.AddOrUpdateWorkflow(workflow), Times.Once);
    }
}