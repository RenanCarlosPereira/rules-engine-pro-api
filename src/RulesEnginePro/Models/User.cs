using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.Models;

[ExcludeFromCodeCoverage]
public record User(string Login, string Name, string Email, string AvatarUrl);