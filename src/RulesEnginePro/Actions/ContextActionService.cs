using Microsoft.Extensions.Options;
using RulesEngine.Models;
using System.Runtime.CompilerServices;

namespace RulesEnginePro.Actions;

internal class ContextActionService(IOptions<ReSettings> options) : IContextActionService
{
    public async IAsyncEnumerable<KeyValuePair<string, Dictionary<string, ContextActionBase.FieldDefinition>>> GetContextActions([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var settings = options.Value;
        foreach (var item in settings.CustomActions ?? [])
        {
            cancellationToken.ThrowIfCancellationRequested();

            var actionName = item.Key;

            var action = item.Value();

            if (action is ContextActionBase context)
                yield return KeyValuePair.Create(actionName, context.ContextDefinition);

            await foreach (var i in BuiltInActionsContext.GetBuiltInDefinitions())
                yield return i;

            await Task.Yield();
        }
    }

}