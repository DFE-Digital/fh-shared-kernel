using FamilyHubs.SharedKernel.Identity.Authorisation;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        /// Refreshes the claims on an interval or when manually trigged
        /// </summary>
        public static Task RefreshClaims(this CookieValidatePrincipalContext context)
        {

            var refreshCookie = context.HttpContext.Request.Cookies.Where(x => x.Key == AuthenticationConstants.RefreshClaimsCookie).FirstOrDefault();
            
            if (refreshCookie.Value != null)
            {
                var expiryTime = long.Parse(refreshCookie.Value);
                if(DateTime.UtcNow.Ticks < expiryTime)
                {
                    return Task.CompletedTask; // cookie still valid, return without refreshing
                }
            }

            context.HttpContext.Response.Cookies.Delete(AuthenticationConstants.RefreshClaimsCookie);
            var claimsValidUntilUtc = DateTime.UtcNow.AddMinutes(5).Ticks.ToString();// This needs to be encypted
            context.Response.Cookies.Append(AuthenticationConstants.RefreshClaimsCookie, claimsValidUntilUtc);

            context.ShouldRenew = true;

            var user = context.Principal;
            var claims = user!.Claims.ToList();
            var emailClaim = claims.Where(x => x.Type == ClaimTypes.Email).First();

            var customClaims = context.HttpContext.RequestServices.GetService<ICustomClaims>();
            var refreshedClaims = customClaims?.RefreshClaims(emailClaim.Value, claims).GetAwaiter().GetResult();

            var newIdentity = new ClaimsIdentity(refreshedClaims, "Cookie");
            context.ReplacePrincipal(new System.Security.Claims.ClaimsPrincipal(newIdentity));

            return Task.CompletedTask;
        }

    }
}
