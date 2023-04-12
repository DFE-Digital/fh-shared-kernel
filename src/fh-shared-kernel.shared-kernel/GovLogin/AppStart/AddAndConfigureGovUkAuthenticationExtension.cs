using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(
            this IServiceCollection services, IConfiguration configuration, string authenticationCookieName, Type customClaims)
        {
            var config = configuration.GetGovUkOidcConfiguration(); 
            if(config == null)
            {
                throw new ArgumentNullException(nameof(GovUkOidcConfiguration), "Could not get Section GovUkOidcConfiguration from configuration");
            }

            services.AddServiceRegistration(configuration, customClaims);
            if (config.StubAuthentication.UseStubAuthentication)
            {
                services.AddEmployerStubAuthentication($"{authenticationCookieName}.stub", config);
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration, authenticationCookieName, config.Urls.SignedOutRedirect);
            }

        }
    }
}
