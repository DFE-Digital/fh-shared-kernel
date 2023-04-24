using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Authentication.Gov;
using FamilyHubs.SharedKernel.Identity.Authentication.Stub;
using Microsoft.AspNetCore.Builder;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    public static class WebApplicationExtenstions
    {
        public static WebApplication UseGovLoginAuthentication(this WebApplication webApplication)
        {
            var config = webApplication.Configuration.GetGovUkOidcConfiguration();
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();
            if (config.StubAuthentication.UseStubAuthentication)
            {
                webApplication.UseMiddleware<StubAccountMiddleware>();
            }
            else
            {
                webApplication.UseMiddleware<AccountMiddleware>();
            }

            return webApplication;
        }
    }
}
