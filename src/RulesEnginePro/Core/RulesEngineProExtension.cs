using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using RulesEnginePro.Actions;
using RulesEnginePro.Models;
using RulesEnginePro.MongoDb;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text.Json.Nodes;

namespace RulesEnginePro.Core;

public static class RulesEngineProExtension
{
    public static IServiceCollection AddRulesEnginePro(this IServiceCollection services)
    {
        services.ConfigureOptions<ReSettingsConfigureOptions>();

        services.AddMongoDb();
        services.AddContextAction();

        services.AddSingleton<IRulesEngine>((sp) =>
        {
            var reSettings = sp.GetRequiredService<IOptions<ReSettings>>().Value;
            return new RulesEngine.RulesEngine(reSettings);
        });
        services.AddSingleton<IRulesEnginePro, RulesEnginePro>();

        return services;
    }

    public static JsonObject GenerateSchema(this IEnumerable<string> identifiers)
    {
        var root = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject()
        };

        if (root["properties"] is not JsonObject properties)
            return root;

        foreach (var id in identifiers)
        {
            if (string.IsNullOrWhiteSpace(id))
                continue;

            var parts = id.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
                continue;

            JsonObject current = properties;

            for (int i = 0; i < parts.Length; i++)
            {
                var key = parts[i];

                if (current[key] is not JsonObject child)
                {
                    child = [];
                    current[key] = child;
                }

                if (i == parts.Length - 1)
                {
                    child["title"] = key;

                    if (child["type"] is not JsonArray)
                    {
                        child["type"] = new JsonArray("null");
                    }
                }
                else
                {
                    child["type"] = "object";

                    if (child["properties"] is not JsonObject nestedProps)
                    {
                        nestedProps = [];
                        child["properties"] = nestedProps;
                    }

                    current = nestedProps;
                }
            }
        }

        return root;
    }
}