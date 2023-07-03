using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity
{
    public static class HttpContextExtensions
    {
        public static string GetBearerToken(this HttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey(AuthenticationConstants.BearerToken))
                return string.Empty;

            var token = httpContext.Items[AuthenticationConstants.BearerToken] as string;

            if(token == null) 
                return string.Empty;

            return token;

        }

        public static async Task<SignOutResult> GovSignOut(this HttpContext httpContext)
        {
            var idToken = await httpContext.GetTokenAsync(AuthenticationConstants.IdToken);

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add(AuthenticationConstants.IdToken, idToken);

            string[] schemes = { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };
            return new SignOutResult(schemes, authenticationProperties );
        }

        public static string GetUrlQueryValue(this HttpContext httpContext, string key)
        {
            var value = httpContext.Request.Query[key].First();

            if(string.IsNullOrEmpty(value))
            {
                throw new Exception($"{key} not found in url query parameters");
            }

            return value;
        }

        public static FamilyHubsUser GetFamilyHubsUser(this HttpContext httpContext)
        {
            var user = new FamilyHubsUser
            {
                Role = GetRole(httpContext),
                OrganisationId = GetClaimValue(httpContext, FamilyHubsClaimTypes.OrganisationId),
                AccountId = GetClaimValue(httpContext, FamilyHubsClaimTypes.AccountId),
                AccountStatus = GetClaimValue(httpContext, FamilyHubsClaimTypes.AccountStatus),
                FullName = GetClaimValue(httpContext, FamilyHubsClaimTypes.FullName),
                LoginTime = GetDataTimeClaimValue(httpContext, FamilyHubsClaimTypes.LoginTime),
                Email = GetClaimValue(httpContext, ClaimTypes.Email),
                PhoneNumber = GetClaimValue(httpContext, FamilyHubsClaimTypes.PhoneNumber)
            };

            return user;
        }

        public static bool IsUserLoggedIn(this HttpContext httpContext)
        {
            return httpContext.User?.Identity?.IsAuthenticated == true;
        }

        public static bool IsUserDfeAdmin(this HttpContext httpContext)
        {
            return GetRole(httpContext) == RoleTypes.DfeAdmin;
        }

        public static bool IsUserLaManager(this HttpContext httpContext)
        {
            var role = GetRole(httpContext);

            if(role == RoleTypes.LaManager || role == RoleTypes.LaDualRole)
            {
                return true;
            }

            return false;
        }

        public static long GetUserOrganisationId(this HttpContext httpContext)
        {
            var organisationId = GetClaimValue(httpContext, FamilyHubsClaimTypes.OrganisationId);

            if(long.TryParse(organisationId, out var result))
            {
                return result;
            }

            throw new Exception("Could not parse OrganisationId from claim");
        }

        private static string GetClaimValue(HttpContext httpContext, string key)
        {

            var claim = httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == key);
            if (claim != null)
            {
                return claim.Value;
            }

            return string.Empty;
        }

        private static DateTime? GetDataTimeClaimValue(HttpContext httpContext, string key)
        {

            var claim = httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == FamilyHubsClaimTypes.LoginTime);


            if (claim != null && long.TryParse(claim.Value, out var utcNumber))
            {
                return new DateTime(utcNumber);
            }

            return null;
        }

        private static string GetRole(HttpContext httpContext)
        {
            var role = GetClaimValue(httpContext, FamilyHubsClaimTypes.Role);
            if (string.IsNullOrEmpty(role))
            {
                role = GetClaimValue(httpContext, ClaimTypes.Role);
            }

            return role;
        }

    }
}
