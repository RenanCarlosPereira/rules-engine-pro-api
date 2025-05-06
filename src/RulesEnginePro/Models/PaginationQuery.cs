using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.Models;

[ExcludeFromCodeCoverage]
public record PaginationQuery(int Skip = 0, int Take = 50);