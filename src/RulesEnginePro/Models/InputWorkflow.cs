using System.Text.Json;

namespace RulesEnginePro.Models;

public record InputWorkflow(JsonElement Inputs, WorkflowData Workflow);