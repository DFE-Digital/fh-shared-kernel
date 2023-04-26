using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Gov
{
    public class AccountMiddleware : AccountMiddlewareBase
    {
        private readonly RequestDelegate _next;
        private readonly GovUkOidcConfiguration _configuration;

        public AccountMiddleware(RequestDelegate next, GovUkOidcConfiguration configuration): base(configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSignOut(context))
            {
                await SignOut(context);
                return;
            }

            SetBearerToken(context);
            await _next(context);
        }

        protected override string GetPrivateKey()
        {
            if (_configuration.StubAuthentication.UseStubAuthentication)
            {
                return _configuration.StubAuthentication.PrivateKey;
            }

            if (string.IsNullOrEmpty(_configuration.Oidc.PrivateKey))
            {
                throw new ArgumentNullException("Configuration must contain private key to generate a bearer token");
            }

            return _configuration.Oidc.PrivateKey;
        }

        private async Task SignOut(HttpContext httpContext)
        {
            var idToken = await httpContext.GetTokenAsync(AuthenticationConstants.IdToken);
            var postLogOutUrl = HttpUtility.UrlEncode($"https://{httpContext.Request.Host}{AuthenticationConstants.AccountLogoutCallback}");
            var logoutRedirect = $"{_configuration.Oidc.BaseUrl}/logout?id_token_hint={idToken}&post_logout_redirect_uri={postLogOutUrl}";
            httpContext.Response.Redirect(logoutRedirect);
        }
    }
}
