using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RulesEnginePro.MongoDb;

internal class MongoDbConfigureOptions(IConfiguration configuration) : IConfigureOptions<MongoDbOptions>
{
    public void Configure(MongoDbOptions options)
    {
        configuration.GetSection("MongoDb").Bind(options);
    }
}
