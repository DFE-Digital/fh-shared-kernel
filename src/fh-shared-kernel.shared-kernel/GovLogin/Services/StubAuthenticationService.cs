using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public interface IStubAuthenticationService
    {
        void AddStubEmployerAuth(IResponseCookies cookies, StubAuthUserDetails model);
    }

    public class StubAuthenticationService : IStubAuthenticationService
    {
        private readonly GovUkOidcConfiguration _configuration;

        public StubAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration.GetGovUkOidcConfiguration();
        }

        public void AddStubEmployerAuth(IResponseCookies cookies, StubAuthUserDetails model)
        {
            if (!_configuration.StubAuthentication.UseStubAuthentication)
            {
                return;
            }

            var authCookie = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                Path = "/",
                Domain = _configuration.StubAuthentication.StubDomain,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            };
            cookies.Append(_configuration.StubAuthentication.AuthCookieName, JsonConvert.SerializeObject(model), authCookie);


        }
    }
}
