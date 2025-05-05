using RulesEngine.Actions;
using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.Actions;

[ExcludeFromCodeCoverage]
public abstract class ContextActionBase : ActionBase
{
    public Dictionary<string, FieldDefinition> ContextDefinition => BuildContextDefinition();

  
    protected abstract Dictionary<string, FieldDefinition> BuildContextDefinition();

    public class FieldDefinition
    {
        public string Type { get; set; } = "string";
        public string? Label { get; set; }
        public object? DefaultValue { get; set; }
        public bool Required { get; set; } = false;
        public Dictionary<string, object>? Metadata { get; set; }
    }
}