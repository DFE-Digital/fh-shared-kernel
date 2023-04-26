using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Stub
{
    public class StubAccountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GovUkOidcConfiguration _configuration;

        public StubAccountMiddleware(RequestDelegate next, GovUkOidcConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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

        private void SetBearerToken(HttpContext httpContext)
        {
            var user = httpContext.User;
            if (!IsUserAuthenticated(user))
                return;

            var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(GetPrivateKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: user.Claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(_configuration.ExpiryInMinutes)
                );

            httpContext.Items.Add(AuthenticationConstants.BearerToken, new JwtSecurityTokenHandler().WriteToken(token));
        }

        private string GetPrivateKey()
        {
            return _configuration.StubAuthentication.PrivateKey;
        }

        private static bool IsUserAuthenticated(ClaimsPrincipal? user)
        {
            if (user == null) return false;

            if (user.Identity == null) return false;

            return user.Identity.IsAuthenticated;
        }

        private static bool ShouldCompleteLogin(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains(StubConstants.RoleSelectedPath))
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
                throw new Exception("Invalid user selected");

            var json = JsonConvert.SerializeObject(user);

            if (string.IsNullOrWhiteSpace(_configuration.CookieName))
                throw new Exception($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            context.Response.Cookies.Append(_configuration.CookieName, json);

            var redirectUrl = context.GetUrlQueryValue("redirect");
            context.Response.Redirect(redirectUrl);

        }
    }
}
