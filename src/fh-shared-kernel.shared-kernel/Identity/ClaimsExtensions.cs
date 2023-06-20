using FamilyHubs.SharedKernel.Identity.Authorisation;
using FamilyHubs.SharedKernel.Identity.Models;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity
{
    public static class ClaimsExtensions
    {
        /// <summary>
        ///  Role claim is required twice, FamilyHubsClaimTypes.Role is the correct format to be sent in the bearertoken
        ///  ClaimTypes.Role is the correct format for IdentityPrinciple
        /// </summary>
        public static void AddRoleClaim(this List<Claim> claims)
        {
            var claim = claims.FirstOrDefault(x => x.Type == FamilyHubsClaimTypes.Role);

            if (claim == null)
            {
                return;
            }

            claims.Add(new Claim(ClaimTypes.Role, claim.Value));
        }

        public static List<Claim> ConvertToSecurityClaim(this List<AccountClaim>? accountClaims)
        {
            var claims = new List<Claim>();
            if(accountClaims == null)
            {
                return claims;
            }

            foreach (var claim in accountClaims!)
            {
                if (!string.IsNullOrEmpty(claim.Name) && claim.Value != null)
                {
                    claims.Add(new Claim(claim.Name, claim.Value));
                }
            }
            claims.AddRoleClaim();

            return claims;
        }
    }
}
