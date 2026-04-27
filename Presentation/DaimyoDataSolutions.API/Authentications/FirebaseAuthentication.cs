using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class FirebaseAuthentication
{
    private readonly RequestDelegate _next;

    public FirebaseAuthentication(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing Authorization header");
            return;
        }

        if (!authHeader.ToString().StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Authorization header format");
            return;
        }

        var token = authHeader.ToString().Substring("Bearer ".Length).Trim();

        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            context.Items["UserId"] = decodedToken.Uid; // You can pass UID to later handlers
        }
        catch (FirebaseAuthException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync($"Invalid Firebase token: {ex.Message}");
            return;
        }

        await _next(context);
    }
}
