using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;

namespace RulesEnginePro.Api;

public static class AuthenticationExtention
{
    public static IServiceCollection AddGithubAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "Auth0";
        })
            .AddCookie()
            .AddOAuth("Auth0", options =>
            {
                options.ClientId = configuration["Auth0:ClientId"]!;
                options.ClientSecret = configuration["Auth0:ClientSecret"]!;
                options.CallbackPath = new PathString("/signin-auth0");

                options.AuthorizationEndpoint = $"https://{configuration["Auth0:Domain"]}/authorize";
                options.TokenEndpoint = $"https://{configuration["Auth0:Domain"]}/oauth/token";
                options.UserInformationEndpoint = $"https://{configuration["Auth0:Domain"]}/userinfo";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request);
                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user.RootElement);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}