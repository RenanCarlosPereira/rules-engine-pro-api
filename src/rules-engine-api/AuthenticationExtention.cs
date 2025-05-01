using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RulesEngine.Api;

public static class AuthenticationExtention
{
    public static IServiceCollection AddGithubAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie()
        .AddGitHub(options =>
        {
            options.ClientId = configuration["Authentication:GitHub:ClientId"] ?? throw new ArgumentException("ClientId cannot be null");
            options.ClientSecret = configuration["Authentication:GitHub:ClientSecret"] ?? throw new ArgumentException("ClientSecret cannot be null"); ;
        });

        return services;
    }
}