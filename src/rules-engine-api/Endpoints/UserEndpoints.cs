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
        app.MapGet("/login", async (
            HttpContext context,
            [FromServices] IConfiguration configuration) =>
        {
            var redirectUri = configuration.GetValue<string>("RedirectUri");
            await context.ChallengeAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        })
        .WithName("Login")
        .WithTags("Authentication")
        .Produces(StatusCodes.Status302Found)
        .WithOpenApi(op =>
        {
            op.Summary = "Initiate login with Auth0";
            op.Description = "Redirects the user to Auth0 for authentication. After login, the user is redirected back to the application.";
            return op;
        });

        app.MapGet("/logout", async context =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.StatusCode = 200;
        })
        .WithName("Logout")
        .WithTags("Authentication")
        .WithOpenApi(op =>
        {
            op.Summary = "Logout the current user";
            op.Description = "Signs the user out of the current session using cookie-based authentication.";
            return op;
        });

        app.MapGet("/me", [Authorize] (HttpContext context) =>
        {
            var user = context.GetUser();
            return Results.Ok(user);
        })
        .WithName("GetCurrentUser")
        .WithTags("Authentication")
        .Produces<User>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(op =>
        {
            op.Summary = "Get current authenticated user";
            op.Description = "Returns information about the currently logged-in user based on claims from Auth0.";
            return op;
        });

        return app;
    }

    public static User GetUser(this HttpContext context)
    {
        var claims = context.User;

        var name = claims.Identity?.Name ?? string.Empty;
        var email = claims.FindFirst(ClaimTypes.Email)?.Value ?? claims.FindFirst("email")?.Value ?? string.Empty;
        var login = claims.FindFirst("nickname")?.Value ?? string.Empty;
        var avatar = claims.FindFirst("picture")?.Value ?? string.Empty;

        return new User(login, name, email, avatar);
    }
}