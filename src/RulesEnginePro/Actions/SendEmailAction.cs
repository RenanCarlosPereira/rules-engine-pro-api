using RulesEngine.Actions;
using RulesEngine.Models;

namespace RulesEnginePro.Actions;

public class SendEmailAction : ContextActionBase
{
    public static readonly string Name = "SendEmail";

    protected override Dictionary<string, FieldDefinition> BuildContextDefinition()
    {
        return new Dictionary<string, FieldDefinition>
        {
            ["recipient"] = new FieldDefinition
            {
                Type = "string",
                Label = "Recipient Email",
                Required = true
            },
            ["subject"] = new FieldDefinition
            {
                Type = "string",
                Label = "Email Subject",
                DefaultValue = "Hello!"
            },
            ["body"] = new FieldDefinition
            {
                Type = "string",
                Label = "Email Body"
            },
            ["test"] = new FieldDefinition
            {
                Type = "object",
                Label = "test"
            }
        };
    }

    public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
    {
        throw new NotImplementedException();
    }
}