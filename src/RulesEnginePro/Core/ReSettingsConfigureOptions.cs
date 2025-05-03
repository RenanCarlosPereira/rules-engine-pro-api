using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RulesEngine.Models;
using RulesEnginePro.Actions;

namespace RulesEnginePro.Core;

internal class ReSettingsConfigureOptions(IConfiguration configuration, IServiceProvider serviceProvider) : IConfigureOptions<ReSettings>
{
    public void Configure(ReSettings options)
    {
        configuration.GetSection(nameof(ReSettings)).Bind(options);
        options.CustomTypes = [typeof(Utils)];
        options.CustomActions = new Dictionary<string, Func<RulesEngine.Actions.ActionBase>>()
        {
            { "SendEmail", serviceProvider.GetRequiredService<SendEmailAction> }
        };
    }
}