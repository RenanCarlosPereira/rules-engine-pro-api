using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RulesEnginePro.Actions;
using RulesEnginePro.Api.Tests.Endpoints;
using RulesEnginePro.Core;

namespace RulesEnginePro.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IWorkflowRepository> WorkflowRepositoryMock { get; } = new();
    public Mock<IRulesEnginePro> RulesEngineMock { get; } = new();
    public Mock<IContextActionService> ContextActionServiceMock = new();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(WorkflowRepositoryMock.Object);
            services.AddSingleton(RulesEngineMock.Object);
            services.AddSingleton(ContextActionServiceMock.Object);
            services.AddSingleton(TimeProvider.System);
            
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RedirectUri"] = "http://localhost/callback"
                }).Build());

        });
    }
}
