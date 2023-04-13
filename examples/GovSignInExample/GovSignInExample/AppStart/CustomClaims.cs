using FamilyHubs.SharedKernel.GovLogin.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace GovSignInExample.AppStart
{
    public class CustomClaims : ICustomClaims
    {
        public Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var value = tokenValidatedContext?.Principal?.Identities.First().Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

            var claims = new List<Claim>
                {
                    new Claim("EmployerAccountId",$"ABC123-{value}"),
                    new Claim(ClaimTypes.AuthorizationDecision,$"Suspended")
                };

            return Task.FromResult(claims.AsEnumerable());
        }
    }
}
