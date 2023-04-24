using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Models;
using FamilyHubs.SharedKernel.Identity.Authorisation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Stub
{
    internal class StubAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GovUkOidcConfiguration _configuration;

        public StubAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ICustomClaims customClaims,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            _customClaims = customClaims;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration.GetGovUkOidcConfiguration();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrWhiteSpace(_configuration.CookieName))
                throw new Exception($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            var json = JsonConvert.SerializeObject(_configuration.StubAuthentication.GovUkUser);
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(_configuration.CookieName, json);

            var request = _httpContextAccessor.HttpContext!.Request;
            var redirect = $"https://{request.Host}{request.Path}{request.QueryString}";

            _httpContextAccessor.HttpContext!.Response.Redirect(redirect);

            return Task.CompletedTask;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(_configuration.CookieName))
                throw new Exception($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            var cookieJson = _httpContextAccessor.HttpContext!.Request.Cookies[_configuration.CookieName];

            if (!TryGetClaimsFromCookie(cookieJson, out var claims))
            {
                return AuthenticateResult.Fail("Cookie not found");
            }

            var identity = new ClaimsIdentity(claims, "Employer-stub");
            var principal = new ClaimsPrincipal(identity);

            if (_customClaims != null)
            {
                var additionalClaims = await _customClaims.GetClaims(new TokenValidatedContext(_httpContextAccessor.HttpContext, Scheme, new OpenIdConnectOptions(), principal, new AuthenticationProperties()));
                claims.AddRange(additionalClaims);
                principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Employer-stub"));
            }

            var ticket = new AuthenticationTicket(principal, "Employer-stub");

            var result = AuthenticateResult.Success(ticket);

            return result;
        }

        private static bool TryGetClaimsFromCookie(string? cookieJson, out List<Claim> claims)
        {
            claims = new List<Claim>();

            if (string.IsNullOrEmpty(cookieJson))
            {
                return false;
            }

            var authCookieValue = JsonConvert.DeserializeObject<GovUkUser>(cookieJson);

            if (authCookieValue == null)
            {
                return false;
            }

            claims.Add(new Claim(ClaimTypes.Email, authCookieValue.Email));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, authCookieValue.Sub));

            return true;
        }
    }
}
