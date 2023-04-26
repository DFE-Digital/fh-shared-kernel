using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Http;

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
    }
}
