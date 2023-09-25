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
        internal static string AppHost { get; set; } = string.Empty;

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
            var value = httpContext.Request.Query[key][0];

            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{key} not found in url query parameters");
            }

            return value;
        }

        public static FamilyHubsUser GetFamilyHubsUser(this HttpContext httpContext)
        {
            var user = new FamilyHubsUser
            {
                Role = httpContext.GetRole(),
                OrganisationId = GetClaimValue(httpContext, FamilyHubsClaimTypes.OrganisationId),
                AccountId = GetClaimValue(httpContext, FamilyHubsClaimTypes.AccountId),
                AccountStatus = GetClaimValue(httpContext, FamilyHubsClaimTypes.AccountStatus),
                FullName = GetClaimValue(httpContext, FamilyHubsClaimTypes.FullName),
                ClaimsValidTillTime = GetDateTimeClaimValue(httpContext, FamilyHubsClaimTypes.ClaimsValidTillTime),
                Email = GetClaimValue(httpContext, ClaimTypes.Email),
                PhoneNumber = GetClaimValue(httpContext, FamilyHubsClaimTypes.PhoneNumber),
                TermsAndConditionsAccepted = TermsAndConditionsAccepted(httpContext)
            };

            return user;
        }

        public static bool IsUserLoggedIn(this HttpContext httpContext)
        {
            return httpContext.User?.Identity?.IsAuthenticated == true;
        }

        public static bool IsUserDfeAdmin(this HttpContext httpContext)
        {
            return httpContext.GetRole() == RoleTypes.DfeAdmin;
        }

        public static bool IsUserLaManager(this HttpContext httpContext)
        {
            var role = httpContext.GetRole();

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

            throw new ArgumentException("Could not parse OrganisationId from claim");
        }

        public static bool TermsAndConditionsAccepted(this HttpContext httpContext)
        {
            var termsAndConditionsAccepted = GetClaimValue(httpContext, $"{FamilyHubsClaimTypes.TermsAndConditionsAccepted}-{AppHost}");

            if (string.IsNullOrEmpty(termsAndConditionsAccepted))
                return false;

            return true;
        }

        public static string GetRole(this HttpContext httpContext)
        {
            var role = GetClaimValue(httpContext, FamilyHubsClaimTypes.Role);
            if (string.IsNullOrEmpty(role))
            {
                role = GetClaimValue(httpContext, ClaimTypes.Role);
            }

            return role;
        }

        /// <summary>
        /// Effects take place on next request
        /// </summary>
        public static void RefreshClaims(this HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append(AuthenticationConstants.RefreshClaimsCookie, "true");
        }

        public static string GetClaimValue(this HttpContext httpContext, string key)
        {
            var claim = httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == key);
            if (claim != null)
            {
                return claim.Value;
            }

            return string.Empty;
        }

        private static DateTime? GetDateTimeClaimValue(HttpContext httpContext, string key)
        {
            var claim = httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == key);

            if (claim != null && long.TryParse(claim.Value, out var utcNumber))
            {
                return new DateTime(utcNumber, DateTimeKind.Utc);
            }

            return null;
        }
    }
}
