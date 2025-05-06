using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RulesEnginePro.Models;

[ExcludeFromCodeCoverage]
public record InputWorkflow(JsonElement Inputs, WorkflowData Workflow);