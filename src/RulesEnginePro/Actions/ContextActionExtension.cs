using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine.ExpressionBuilders;
using RulesEngine.Models;
using RulesEnginePro.Core;

namespace RulesEnginePro.Actions;

public static class ContextActionExtension
{
    public static IServiceCollection AddContextAction(this IServiceCollection services)
    {
        services.ConfigureOptions<ReSettingsConfigureOptions>();

        services.AddTransient<SendEmailAction>();

        services.AddSingleton<IContextActionService, ContextActionService>();
        services.AddSingleton(sp =>
        {
            var reSettings = sp.GetRequiredService<IOptions<ReSettings>>().Value;
            return new RuleExpressionParser(reSettings);
        });


        return services;
    }
}