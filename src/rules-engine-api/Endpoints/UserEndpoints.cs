using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RulesEnginePro.Models;
using System.Security.Claims;

namespace RulesEngine.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/login", async (HttpContext context, [FromServices]IConfiguration configuration) =>
        {
            var redirectUri = configuration.GetValue("RedirectUri", "https://rules-engine-pro-ui.onrender.com");
            await context.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        });

        app.MapGet("/logout", async context =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.StatusCode = 200;
        });

        app.MapGet("/me", [Authorize] (HttpContext context) =>
        {
            var user = context.User;

            var login = user.FindFirst("urn:github:login")?.Value ?? string.Empty;
            var name = user.FindFirst("urn:github:name")?.Value ?? string.Empty;
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            var avatar = user.FindFirst("urn:github:avatar")?.Value ?? string.Empty;

            var githubUser = new GitHubUser(login, name, email, avatar);

            return Results.Ok(githubUser);
        });

        return app;
    }
}