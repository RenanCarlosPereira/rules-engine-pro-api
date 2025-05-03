namespace RulesEnginePro.Models;

public record Auditing(User User, DateTimeOffset LastModified);