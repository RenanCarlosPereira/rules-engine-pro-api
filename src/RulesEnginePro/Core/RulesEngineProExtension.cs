using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine.Interfaces;
using RulesEngine.Models;
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

    public static IEnumerable<string> ExtractIdentifiers(this Workflow workflow)
    {
        var config = new ParsingConfig();
        var identifiers = new HashSet<string>();

        foreach (var expr in workflow.GetAllExpressions())
        {
            try
            {
                var parser = new TextParser(config, expr);

                while (parser.CurrentToken.Id != TokenId.End)
                {
                    if (parser.CurrentToken.Id == TokenId.Identifier)
                    {
                        var parts = new List<string> { parser.CurrentToken.Text };
                        parser.NextToken();

                        bool hasDot = false;

                        while (parser.CurrentToken.Id == TokenId.Dot)
                        {
                            parser.NextToken();

                            if (parser.CurrentToken.Id == TokenId.Identifier)
                            {
                                var nextText = parser.CurrentToken.Text;
                                parser.NextToken();
                                if (parser.CurrentToken.Id == TokenId.OpenParen)
                                {
                                    break;
                                }

                                parts.Add(".");
                                parts.Add(nextText);
                                hasDot = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (hasDot)
                        {
                            identifiers.Add(string.Concat(parts));
                        }
                    }
                    else
                    {
                        parser.NextToken();
                    }
                }
            }
            catch
            {
                // Skip invalid expressions
            }
        }

        return identifiers;
    }

    private static IEnumerable<string> GetAllExpressions(this Workflow workflow)
    {
        if (workflow?.Rules == null)
            yield break;

        foreach (var globalParam in workflow.GlobalParams ?? [])
            yield return globalParam.Expression;

        foreach (var expression in workflow.Rules.SelectMany(GetExpressionsFromRule))
            yield return expression;
    }

    private static IEnumerable<string> GetExpressionsFromRule(Rule rule)
    {
        if (rule == null)
            yield break;

        if (!string.IsNullOrWhiteSpace(rule.Expression))
            yield return rule.Expression;

        foreach (var localParam in rule.LocalParams ?? [])
            yield return localParam.Expression;

        foreach (var nested in rule.Rules?.SelectMany(GetExpressionsFromRule) ?? [])
            yield return nested;
    }
}