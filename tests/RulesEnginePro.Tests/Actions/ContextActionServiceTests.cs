using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RulesEngine.Actions;
using RulesEngine.Models;
using RulesEnginePro.Actions;

namespace RulesEnginePro.Tests.Actions;

public class ContextActionServiceTests
{
    private readonly Faker _faker = new();

    public class TestContextAction : ContextActionBase
    {
        public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            return ValueTask.FromResult<object>("result");
        }

        protected override Dictionary<string, FieldDefinition> BuildContextDefinition() => new()
        {
            ["TestField"] = new FieldDefinition
            {
                Type = "string",
                Label = "Test Label",
                Required = true
            }
        };
    }

    [Fact]
    public async Task GetContextActions_ShouldReturnCustomAndBuiltInActions()
    {
        // Arrange
        var customActionName = _faker.Random.Word();
        var customActions = new Dictionary<string, Func<ActionBase>>
        {
            [customActionName] = () => new TestContextAction()
        };

        var settings = new ReSettings { CustomActions = customActions };
        var optionsMock = new Mock<IOptions<ReSettings>>();
        optionsMock.Setup(o => o.Value).Returns(settings);

        var service = new ContextActionService(optionsMock.Object);

        // Act
        var results = new List<KeyValuePair<string, Dictionary<string, ContextActionBase.FieldDefinition>>>();
        await foreach (var item in service.GetContextActions(CancellationToken.None))
        {
            results.Add(item);
        }

        // Assert
        results.Should().Contain(x => x.Key == customActionName);
        results.Should().Contain(x => x.Key == "OutputExpression");
        results.Should().Contain(x => x.Key == "EvaluateRule");

        results.First(x => x.Key == customActionName).Value.Should().ContainKey("TestField");
    }

    [Fact]
    public async Task GetContextActions_ShouldRespectCancellation()
    {
        // Arrange
        var customActions = new Dictionary<string, Func<ActionBase>>
        {
            ["Test"] = () => new TestContextAction()
        };

        var settings = new ReSettings { CustomActions = customActions };
        var optionsMock = new Mock<IOptions<ReSettings>>();
        optionsMock.Setup(o => o.Value).Returns(settings);

        var service = new ContextActionService(optionsMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        var enumerator = service.GetContextActions(cts.Token).GetAsyncEnumerator(cts.Token);
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
    }
}