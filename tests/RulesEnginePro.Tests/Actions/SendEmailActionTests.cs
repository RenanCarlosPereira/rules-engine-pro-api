using FluentAssertions;
using RulesEngine.Actions;
using RulesEngine.Models;
using RulesEnginePro.Actions;
using static RulesEnginePro.Actions.ContextActionBase;

namespace RulesEnginePro.Tests.Actions;

public class SendEmailActionTests
{
    [Fact]
    public void ContextDefinition_ShouldContainAllExpectedFields()
    {
        // Arrange
        var action = new SendEmailAction();

        // Act
        var contextDefinition = action.ContextDefinition;

        // Assert
        contextDefinition.Should().ContainKeys("recipient", "subject", "body", "test");

        contextDefinition["recipient"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "Recipient Email",
            Required = true
        });

        contextDefinition["subject"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "Email Subject",
            DefaultValue = "Hello!"
        });

        contextDefinition["body"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "string",
            Label = "Email Body"
        });

        contextDefinition["test"].Should().BeEquivalentTo(new FieldDefinition
        {
            Type = "object",
            Label = "test"
        });
    }

    [Fact]
    public async Task Run_ShouldReturnEmailSentMessage()
    {
        // Arrange
        var action = new SendEmailAction();
        var context = new ActionContext(new Dictionary<string,object>(), new RuleResultTree());
        var parameters = Array.Empty<RuleParameter>();

        // Act
        var result = await action.Run(context, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Message = "Email sent" });
    }
}