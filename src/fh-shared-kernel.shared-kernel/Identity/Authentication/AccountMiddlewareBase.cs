using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FamilyHubs.SharedKernel.Identity.Authentication
{
    public abstract class AccountMiddlewareBase
    {
        private readonly GovUkOidcConfiguration _configuration;

        protected AccountMiddlewareBase(GovUkOidcConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected bool ShouldSignOut(HttpContext httpContext)
        {
            if (httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.Contains(AuthenticationConstants.SignOutPath, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        protected bool ShouldRedirectToNoClaims(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var isAuthorized = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;

            if (!isAuthorized)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_configuration.Urls.NoClaimsRedirect))
            {
                return false; // If a redirect setting does not exist we dont need to redirect
            }

            if (_configuration.Urls.NoClaimsRedirect.Contains(httpContext.Request.Path))
            {
                return false; // If we are already redirecting to the NoClaimsPage no need to redirect again
            }

            if (!httpContext.IsUserLoggedIn())
            {
                return false; // We only redirect to NoClaims page if user is logged in and doesn't have claims
            }

            var user = httpContext.GetFamilyHubsUser();

            if(string.IsNullOrEmpty(user.Role) || string.IsNullOrEmpty(user.OrganisationId) || string.IsNullOrEmpty(user.FullName))
            {
                return true;  // Missing required claims, redirect to NoClaims page
            }

            return false;
        }

        protected void SetBearerToken(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey(AuthenticationConstants.BearerToken))
                return;

            var user = httpContext.User;
            if (!IsUserAuthenticated(user))
                return;

            var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_configuration.BearerTokenSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: user.Claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(_configuration.ExpiryInMinutes)
                );

            httpContext.Items.Add(AuthenticationConstants.BearerToken, new JwtSecurityTokenHandler().WriteToken(token));
        }

        private static bool IsUserAuthenticated(ClaimsPrincipal? user)
        {
            if (user == null) return false;

            if (user.Identity == null) return false;

            return user.Identity.IsAuthenticated;
        }
    }
}
