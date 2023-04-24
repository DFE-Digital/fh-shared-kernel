using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;


namespace FamilyHubs.SharedKernel.Identity.Authorisation.Stub
{
    public class StubClaims : ICustomClaims
    {
        private List<AccountClaim> _claims;

        public StubClaims(GovUkOidcConfiguration govUkOidcConfiguration)
        {
            _claims = govUkOidcConfiguration.StubAuthentication.StubClaims;
        }

        public Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var claims = new List<Claim>();

            foreach (var claim in _claims)
            {
                claims.Add(new Claim(claim.Name, claim.Value));
            }

            return Task.FromResult(claims.AsEnumerable());
        }
    }
}
