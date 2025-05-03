namespace RulesEnginePro.Actions;

public interface IContextActionService
{
    IAsyncEnumerable<KeyValuePair<string, Dictionary<string, ContextActionBase.FieldDefinition>>> GetContextActions(CancellationToken cancellationToken);
}
