using RulesEngine.Models;
using System.Text.Json;

namespace RulesEnginePro.Models;

public record InputWorkflow(JsonElement Inputs, Workflow Workflow);
