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
            if (!PageRequiresAuthorization(httpContext))
            {
                return false;
            }

            if (string.IsNullOrEmpty(_configuration.Urls.NoClaimsRedirect))
            {
                // If a redirect setting does not exist we don't need to redirect
                return false;
            }

            if (httpContext.Request.Path.Value?.StartsWith(_configuration.Urls.NoClaimsRedirect) == true)
            {
                // If we are already redirecting to the NoClaimsPage no need to redirect again
                return false;
            }

            if (!httpContext.IsUserLoggedIn())
            {
                // We only redirect to NoClaims page if user is logged in and doesn't have claims
                return false;
            }

            var user = httpContext.GetFamilyHubsUser();

            // if role, organisationId or full name is missing redirect to 401 page
            return string.IsNullOrEmpty(user.Role) || string.IsNullOrEmpty(user.OrganisationId) || string.IsNullOrEmpty(user.FullName);
        }

        protected bool ShouldRedirectToTermsAndConditions(HttpContext httpContext)
        {
            if (!PageRequiresAuthorization(httpContext))
            {
                return false;
            }

            if (string.IsNullOrEmpty(_configuration.Urls.TermsAndConditionsRedirect))
            {
                // If a redirect setting does not exist we don't need to redirect
                return false;
            }

            if (httpContext.Request.Path.Value?.StartsWith(_configuration.Urls.TermsAndConditionsRedirect) == true)
            {
                // If we are already redirecting to the TermsAndConditions no need to redirect again
                return false;
            }

            if (!httpContext.IsUserLoggedIn())
            {
                // We only redirect to TermsAndConditions page if user is logged in and doesn't have claims
                return false;
            }

            if (httpContext.TermsAndConditionsAccepted())
            {
                return false;
            }

            return true;
        }

        protected bool PageRequiresAuthorization(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;
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

        protected static bool IsUserAuthenticated(ClaimsPrincipal? user)
        {
            if (user == null) return false;

            if (user.Identity == null) return false;

            return user.Identity.IsAuthenticated;
        }
    }
}
