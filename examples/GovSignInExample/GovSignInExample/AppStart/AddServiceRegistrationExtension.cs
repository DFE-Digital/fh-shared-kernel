using FamilyHubs.SharedKernel.GovLogin.AppStart;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GovSignInExample.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddAndConfigureGovUkAuthentication(configuration, $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth", typeof(CustomClaims));
        }
    }
}
