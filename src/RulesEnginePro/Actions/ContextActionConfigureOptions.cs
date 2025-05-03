using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine.Models;

namespace RulesEnginePro.Actions;

internal class ContextActionConfigureOptions(IServiceProvider provider) : IConfigureOptions<ReSettings>
{
    public void Configure(ReSettings options)
    {
        options.CustomActions = new Dictionary<string, Func<RulesEngine.Actions.ActionBase>>()
        {
            { SendEmailAction.Name, provider.GetRequiredService<SendEmailAction> },
        };
    }
}