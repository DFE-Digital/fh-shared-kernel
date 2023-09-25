using FamilyHubs.SharedKernel.Identity.Authorisation;
using FamilyHubs.SharedKernel.Identity.Exceptions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
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
            var claim = claims.Find(x => x.Type == FamilyHubsClaimTypes.Role);

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
            var user = context.Principal;
            var claims = user!.Claims.ToList();

            if (!ShouldRefreshClaims(context.HttpContext, claims))
            {
                return Task.CompletedTask; // claims still valid, return without refreshing
            }

            context.HttpContext.Response.Cookies.Delete(AuthenticationConstants.RefreshClaimsCookie);
            context.ShouldRenew = true;

            var emailClaim = claims.First(x => x.Type == ClaimTypes.Email);

            var customClaims = context.HttpContext.RequestServices.GetService<ICustomClaims>();
            var refreshedClaims = customClaims?.RefreshClaims(emailClaim.Value, claims).GetAwaiter().GetResult();

            var newIdentity = new ClaimsIdentity(refreshedClaims, "Cookie");
            context.ReplacePrincipal(new ClaimsPrincipal(newIdentity));

            return Task.CompletedTask;
        }

        private static bool ShouldRefreshClaims(HttpContext httpContext, List<Claim>? claims)
        {
            var refreshCookie = httpContext.Request.Cookies
                .FirstOrDefault(x => x.Key == AuthenticationConstants.RefreshClaimsCookie);
            var claim = claims?.Find(x=>x.Type == FamilyHubsClaimTypes.ClaimsValidTillTime);

            if (claim == null)
            {
                throw new ClaimsException($"{FamilyHubsClaimTypes.ClaimsValidTillTime} claim missing from user claims");
            }

            var claimsValidTillTime = long.Parse(claim.Value);

            if (refreshCookie.Value != null || claimsValidTillTime < DateTime.UtcNow.Ticks)
            {
                return true; 
            }

            return false;
        }
    }
}
