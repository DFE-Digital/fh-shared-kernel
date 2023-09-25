using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity.Authorisation.Stub
{
    public class StubClaims : ICustomClaims
    {
        private readonly List<Claim> _claims = new();

        public StubClaims(GovUkOidcConfiguration govUkOidcConfiguration)
        {
            if(govUkOidcConfiguration.StubAuthentication.StubClaims != null)
            {
                foreach (var claim in govUkOidcConfiguration.StubAuthentication.StubClaims)
                {
                    _claims.Add(new Claim(claim.Name, claim.Value));
                }
                _claims.AddRoleClaim();
            }
        }

        public Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            return Task.FromResult(_claims.AsEnumerable());
        }

        public Task<IEnumerable<Claim>> RefreshClaims(string email, List<Claim> currentClaims)
        {
            //  For stub dont refresh claim, just return currentClaims
            return Task.FromResult((IEnumerable<Claim>)currentClaims);
        }
    }
}
