using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.MongoDb;

[ExcludeFromCodeCoverage]
internal class MongoDbOptions
{
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
}