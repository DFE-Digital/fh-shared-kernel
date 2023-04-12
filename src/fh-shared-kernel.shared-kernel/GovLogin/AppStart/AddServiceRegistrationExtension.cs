using FamilyHubs.SharedKernel.GovLogin.Authentication;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration,
            Type customClaims)
        {
            if (!configuration.GetSection(nameof(GovUkOidcConfiguration)).GetChildren().Any())
            {
                throw new ArgumentException(
                    "Cannot find GovUkOidcConfiguration in configuration. Please add a section called GovUkOidcConfiguration with BaseUrl, ClientId and KeyVaultIdentifier properties.");
            }

            services.AddOptions();
            services.AddTransient(typeof(ICustomClaims), customClaims);
            services.AddSingleton(c => c.GetService<IOptions<GovUkOidcConfiguration>>()!.Value);
            services.AddHttpClient<IOidcService, OidcService>();
            services.AddTransient<IAzureIdentityService, AzureIdentityService>();
            services.AddTransient<IJwtSecurityTokenService, JwtSecurityTokenService>();
            services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();
            services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();
        }
    }
}
