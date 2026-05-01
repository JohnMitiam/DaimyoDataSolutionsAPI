using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using DaimyoDataSolutions.Domain.Entities;
//using DaimyoDataSolutions.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DaimyoDataSolutions.API.Authentication
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public FirebaseAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
            : base(options, logger, encoder)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }
        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? authHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.NoResult(); // No token provided
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                // NEW CODE : Ensure user profile exists in our system
                //await EnsureUserProfileExistsAsync(decodedToken.Uid);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    new Claim("firebase_uid", decodedToken.Uid)
                };

                // Add email if present
                if (decodedToken.Claims.TryGetValue("email", out var emailValue) && emailValue != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, emailValue.ToString()!));
                }

                //  EXTRACT ROLE FROM FIREBASE CUSTOM CLAIMS
                // Firebase admin sets custom claims: admin.auth().setCustomUserClaims(uid, { role: 'Admin' })
                if (decodedToken.Claims.TryGetValue("role", out var roleValue) && roleValue != null)
                {
                    var role = roleValue.ToString()!;
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    Logger.LogInformation("User {UserId} authenticated with role: {Role}", decodedToken.Uid, role);
                }
                else
                {
                    //  FALLBACK: Check X-User-Role header (for development/testing only)
                    var allowHeaderOverride = _configuration.GetValue<bool>("Authentication:AllowHeaderOverride", true);

                    if (allowHeaderOverride)
                    {
                        var headerRole = Request.Headers["X-User-Role"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(headerRole))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, headerRole));
                            Logger.LogWarning("User {UserId} role set from X-User-Role header: {Role} (DEV MODE)",
                                decodedToken.Uid, headerRole);
                        }
                        else
                        {
                            // Default role
                            var defaultRole = _configuration.GetValue<string>("Authentication:DefaultRole") ?? "Guest";
                            claims.Add(new Claim(ClaimTypes.Role, defaultRole));
                            Logger.LogInformation("User {UserId} assigned default role: {Role}", decodedToken.Uid, defaultRole);
                        }
                    }
                    else
                    {
                        // Production mode: require role in JWT
                        var defaultRole = _configuration.GetValue<string>("Authentication:DefaultRole") ?? "Guest";
                        claims.Add(new Claim(ClaimTypes.Role, defaultRole));
                        Logger.LogWarning("No role claim found for user {UserId}, assigned default: {Role}",
                            decodedToken.Uid, defaultRole);
                    }
                }

                //  ADD OTHER FIREBASE CLAIMS (excluding duplicates)
                foreach (var kvp in decodedToken.Claims)
                {
                    if (kvp.Key != "role" && kvp.Key != "email" && kvp.Value != null)
                    {
                        claims.Add(new Claim($"firebase_{kvp.Key}", kvp.Value.ToString()!));
                    }
                }

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FirebaseAuthException ex)
            {
                Logger.LogWarning(ex, "Firebase token validation failed: {Message}", ex.Message);
                return AuthenticateResult.Fail($"Invalid Firebase token: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error during authentication");
                return AuthenticateResult.Fail("Authentication failed");
            }
        }
    }
}
