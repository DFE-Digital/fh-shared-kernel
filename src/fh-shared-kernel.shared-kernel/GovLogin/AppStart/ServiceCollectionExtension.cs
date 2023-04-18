using FamilyHubs.SharedKernel.GovLogin.Authentication;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Configures UI to authenticate using Gov Login
        /// </summary>
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


        /// <summary>
        /// Adds a httpclient with user bearer token
        /// </summary>
        /// <param name="name">httpClient Name</param>
        /// <param name="configureClient">For further httpclient configuration</param>
        public static IServiceCollection AddSecureClient(this IServiceCollection serviceCollection, string name, Action<IServiceProvider, HttpClient> configureClient)
        {
            serviceCollection.AddHttpClient(name).ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                configureClient(serviceProvider, httpClient);
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor == null)
                    throw new Exception($"IHttpContextAccessor required for {nameof(AddSecureClient)}");

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {httpContextAccessor.HttpContext!.GetBearerToken()}");
            });


            return serviceCollection;
        }

        /// <summary>
        /// For use in API. Endpoints with [Authorize] attribute with authorize using bearer tokens
        /// </summary>
        public static void AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetGovUkOidcConfiguration();

            var privateKey = config.Oidc.PrivateKey;
            if (string.IsNullOrEmpty(privateKey))
                throw new Exception("PrivateKey must be configured for AddBearerAuthentication");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options => {
                  options.RequireHttpsMetadata = false;

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(privateKey)),
                      ValidateLifetime = true,
                      ValidateAudience = false,
                      ValidateIssuer = false
                  };
              });
        }
    }
}
