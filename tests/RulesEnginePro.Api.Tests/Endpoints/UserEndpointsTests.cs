using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RulesEnginePro.Models;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit;

namespace RulesEnginePro.Api.Tests.Endpoints;

[Collection("IntegrationTests")]
public class UserEndpointsTests(TestFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateAuthenticatedClient();

    [Fact]
    public async Task MeEndpoint_ReturnsUser()
    {
        var response = await _client.GetAsync("/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.Equal("testuser", user?.Name);
    }

    [Fact]
    public async Task LogoutEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/logout");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LoginEndpoint_Redirects()
    {
        var response = await _client.GetAsync("/login");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }
}

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) :
    AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
        [
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
        new Claim("nickname", "tester"),
        new Claim("picture", "http://example.com/avatar.png")
    ], "Test");

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}