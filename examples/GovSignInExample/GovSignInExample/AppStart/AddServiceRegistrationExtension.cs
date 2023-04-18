using FamilyHubs.SharedKernel.GovLogin.AppStart;

namespace GovSignInExample.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAndConfigureGovUkAuthentication(configuration, $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth");
        }
    }
}
