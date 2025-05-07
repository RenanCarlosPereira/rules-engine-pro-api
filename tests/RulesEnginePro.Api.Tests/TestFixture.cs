using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace RulesEnginePro.Api.Tests;

public class TestFixture
{
    public CustomWebApplicationFactory Factory { get; } = new();
    public HttpClient CreateAuthenticatedClient()
    {
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        return client;
    }
}
