using System.Security.Claims;

namespace DaimyoDataSolutions.API.Authentications
{
    public static class AuthenticationExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            var nameIdentifierClaim = principal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
            
            if (nameIdentifierClaim == null)
            {
                return null;
            }

            return nameIdentifierClaim.Value;
        }
    }
}
