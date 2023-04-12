using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    internal static class ConfigureGovUkStubAuthenticationExtension
    {

        public static void AddEmployerStubAuthentication(this IServiceCollection services,
            string authenticationCookieName, GovUkOidcConfiguration config)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(authenticationCookieName, _ => { })
                .AddCookie(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName);
                        c.Response.Cookies.Delete(config.StubAuthentication.AuthCookieName);
                        c.Response.Redirect(config.Urls.SignedOutRedirect);
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthentication(authenticationCookieName).AddAuthenticationCookie(authenticationCookieName);
        }

    }
}
