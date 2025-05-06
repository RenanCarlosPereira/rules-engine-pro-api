using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.Models;

[ExcludeFromCodeCoverage]
public record Auditing(User User, DateTimeOffset LastModified);