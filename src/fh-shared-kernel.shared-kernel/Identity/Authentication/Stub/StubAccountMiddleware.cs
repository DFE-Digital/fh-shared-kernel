using Azure.Core;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Exceptions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Stub
{
    public class StubAccountMiddleware : AccountMiddlewareBase
    {
        private readonly RequestDelegate _next;
        private readonly GovUkOidcConfiguration _configuration;

        public StubAccountMiddleware(
            RequestDelegate next, 
            GovUkOidcConfiguration configuration) : base(configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSignOut(context))
            {
                SignOut(context);
                return;
            }

            if (StubLoginPage.ShouldRedirectToStubLoginPage(context))
            {
                await StubLoginPage.RenderStubLoginPage(context, _configuration);
                return;
            }

            if (ShouldCompleteLogin(context))
            {
                CompleteLogin(context);
                return;
            }

            SetBearerToken(context);
            await _next(context);
        }

        private static bool ShouldCompleteLogin(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains(StubConstants.RoleSelectedPath, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private void CompleteLogin(HttpContext context)
        {
            var userId = context.GetUrlQueryValue("user");

            var user = _configuration.GetStubUsers().First(x => x.User.Email == userId);
            if (user == null)
                throw new InvalidOperationException("Invalid user selected");

            var claims = user.Claims.ConvertToSecurityClaim();
            claims.Add(new Claim(FamilyHubsClaimTypes.ClaimsValidTillTime, DateTime.UtcNow.AddMinutes(_configuration.ClaimsRefreshTimerMinutes).Ticks.ToString() ));//Not actually used in stub mode
            claims.Add(new Claim(ClaimTypes.Email, user.User.Email));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.User.Sub));

            var json = JsonConvert.SerializeObject(new StubClaimsCookie(claims));

            if (string.IsNullOrWhiteSpace(_configuration.CookieName))
                throw new AuthConfigurationException($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");
            context.Response.Cookies.Append(_configuration.CookieName, json);

            var redirectUrl = context.Request.Query["redirect"][0];

            if(string.IsNullOrWhiteSpace(redirectUrl))
            {
                context.Response.Redirect(_configuration.AppHost!);
                return;
            }

            if (!redirectUrl.StartsWith('/'))
            {
                redirectUrl = $"/{redirectUrl}";
            }

            context.Response.Redirect(redirectUrl);

        }
        private void SignOut(HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(_configuration.CookieName))
                throw new AuthConfigurationException($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            httpContext.Response.Cookies.Delete(_configuration.CookieName);
            httpContext.Response.Redirect(_configuration.Urls.SignedOutRedirect);
        }
    }
}
