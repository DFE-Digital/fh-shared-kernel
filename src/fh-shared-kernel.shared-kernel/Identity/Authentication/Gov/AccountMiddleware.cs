using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Gov
{
    public class AccountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GovUkOidcConfiguration _configuration;

        public AccountMiddleware(RequestDelegate next, GovUkOidcConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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

        private static bool IsUserAuthenticated(ClaimsPrincipal? user)
        {
            if (user == null) return false;

            if (user.Identity == null) return false;

            return user.Identity.IsAuthenticated;
        }
    }
}
