using FamilyHubs.SharedKernel.GovLogin.Authentication;
using Microsoft.AspNetCore.Builder;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    public static class WebApplicationExtenstions
    {
        public static WebApplication UseGovLoginAuthentication(this WebApplication webApplication)
        {
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();
            webApplication.UseMiddleware<AccountMiddleware>();
            return webApplication;
        }
    }
}
