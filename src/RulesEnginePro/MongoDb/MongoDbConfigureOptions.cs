using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.MongoDb;

[ExcludeFromCodeCoverage]
internal class MongoDbConfigureOptions(IConfiguration configuration) : IConfigureOptions<MongoDbOptions>
{
    public void Configure(MongoDbOptions options)
    {
        configuration.GetSection("MongoDb").Bind(options);
    }
}