using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RulesEngine.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/login", async (HttpContext context, [FromServices] IConfiguration configuration) =>
        {
            var redirectUri = configuration.GetValue<string>("RedirectUri");
            await context.ChallengeAsync("Auth0", new AuthenticationProperties
            { RedirectUri = redirectUri });
        });

        app.MapGet("/logout", async context =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.StatusCode = 200;
        });

        app.MapGet("/me", (HttpContext context) =>
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                return Results.Ok(new
                {
                    name = context.User.Identity.Name,
                    email = context.User.FindFirst(ClaimTypes.Email)?.Value
                });
            }

            return Results.Unauthorized();
        });

        return app;
    }
}