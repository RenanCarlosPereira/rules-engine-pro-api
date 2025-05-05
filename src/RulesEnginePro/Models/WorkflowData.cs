using RulesEngine.Models;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Dynamic.Core;

namespace RulesEnginePro.Models;

public class WorkflowData : Workflow
{
    public Auditing? Auditing { get; set; }

    public IEnumerable<string> ExtractIdentifiers()
    {
        var config = new ParsingConfig();
        var identifiers = new HashSet<string>();

        foreach (var expr in GetAllExpressions())
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

    private IEnumerable<string> GetAllExpressions()
    {
        if (Rules == null)
            yield break;

        foreach (var globalParam in GlobalParams ?? [])
            yield return globalParam.Expression;

        foreach (var expression in Rules.SelectMany(GetExpressionsFromRule))
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