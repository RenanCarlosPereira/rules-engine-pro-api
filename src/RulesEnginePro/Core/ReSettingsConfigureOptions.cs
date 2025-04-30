using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RulesEngine.Models;

namespace RulesEnginePro.Core;

internal class ReSettingsConfigureOptions(IConfiguration configuration) : IConfigureOptions<ReSettings>
{
    public void Configure(ReSettings options)
    {
        configuration.GetSection(nameof(ReSettings)).Bind(options);
        options.CustomTypes = [typeof(Utils)];
    }
}
