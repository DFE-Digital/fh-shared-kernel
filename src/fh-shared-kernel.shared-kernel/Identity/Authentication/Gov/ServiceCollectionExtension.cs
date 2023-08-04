using FamilyHubs.SharedKernel.GovLogin.AppStart;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace FamilyHubs.SharedKernel.Identity.Authentication.Gov
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddGovUkAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var govUkConfiguration = configuration.GetGovUkOidcConfiguration();
            if (string.IsNullOrWhiteSpace(govUkConfiguration.CookieName))
                throw new AuthConfigurationException($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            services
                .AddAuthentication(options => ConfigureAuthenticationOptions(options))
                .AddOpenIdConnect(options => ConfigureOpenIdConnect(options, govUkConfiguration))
                .AddAuthenticationCookie(govUkConfiguration.CookieName, govUkConfiguration);

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IOidcService, IAzureIdentityService, GovUkOidcConfiguration>(ConfigureToken);
        }

        private static void ConfigureAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

        }

        private static void ConfigureOpenIdConnect(OpenIdConnectOptions options, GovUkOidcConfiguration govUkConfiguration)
        {
            options.ClientId = govUkConfiguration.Oidc.ClientId;
            options.MetadataAddress = $"{govUkConfiguration.Oidc.BaseUrl}/.well-known/openid-configuration";
            options.ResponseType = "code";
            options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
            options.SignedOutRedirectUri = AbsoluteUrlFromPath("/Account/logout-callback", govUkConfiguration);
            options.SignedOutCallbackPath = FullPathFromPath("/Account/logout-callback", govUkConfiguration);
            options.CallbackPath = FullPathFromPath("/Account/login-callback", govUkConfiguration);
            options.ResponseMode = string.Empty;
            options.SaveTokens = true;

            var scopes = "openid email phone".Split(' ');
            options.Scope.Clear();
            foreach (var scope in scopes)
            {
                options.Scope.Add(scope);
            }

            options.Events.OnRedirectToIdentityProvider = c =>
            {
                if (!govUkConfiguration.Oidc.TwoFactorEnabled)
                {
                    // If TwoFactorDisabled this parameter will lower the authenitcation level and not
                    // require a two-factor code
                    c.ProtocolMessage.SetParameter("vtr", "[\"Cl\"]");
                }

                c.ProtocolMessage.SetParameter("prompt", "login");

                c.ProtocolMessage.RedirectUri =
                    AbsoluteUrlFromPath("/Account/login-callback", govUkConfiguration);
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToIdentityProviderForSignOut = c =>
            {
                c.ProtocolMessage.RedirectUri =
                    AbsoluteUrlFromPath("/Account/logout-callback", govUkConfiguration);
                return Task.CompletedTask;
            };

            options.Events.OnRemoteFailure = c =>
            {
                if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                {
                    // the homepage seems a sensible place to redirect, even if user is signing-in to a path routed page
                    c.Response.Redirect("/");
                    c.HandleResponse();
                }

                return Task.CompletedTask;
            };

            options.Events.OnSignedOutCallbackRedirect = c =>
            {
                c.Response.Cookies.Delete(govUkConfiguration.CookieName!);
                c.Response.Redirect(AbsoluteUrl(govUkConfiguration.Urls.SignedOutRedirect, govUkConfiguration));
                c.HandleResponse();
                return Task.CompletedTask;
            };
        }

        private static string FullPathFromPath(string path, GovUkOidcConfiguration config)
        {
            return $"{config.AppBasePath}{path}";
        }

        private static string AbsoluteUrlFromPath(string path, GovUkOidcConfiguration config)
        {
            return $"{config.AppHost}{FullPathFromPath(path, config)}";
        }

        //todo: move somewhere central
        //todo: use in more places?
        // support absolute and relative urls in config
        private static string AbsoluteUrl(string relativeOrAbsoluteUrl, GovUkOidcConfiguration config)
        {
            if (new Uri(relativeOrAbsoluteUrl).IsAbsoluteUri)
            {
                return relativeOrAbsoluteUrl;
            }

            return AbsoluteUrlFromPath(relativeOrAbsoluteUrl, config);
        }

        private static void ConfigureToken(OpenIdConnectOptions options, IOidcService oidcService, IAzureIdentityService azureIdentityService, GovUkOidcConfiguration config)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                AuthenticationType = "private_key_jwt",
                IssuerSigningKey = GetIssuerSigningKey(config, azureIdentityService),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                SaveSigninToken = true
            };
            options.Events.OnAuthorizationCodeReceived = async (ctx) =>
            {
                var token = await oidcService.GetToken(ctx.TokenEndpointRequest!);
                if (token?.AccessToken != null && token.IdToken != null)
                {
                    ctx.HandleCodeRedemption(token.AccessToken, token.IdToken);
                }
            };

            options.Events.OnTokenValidated = async ctx => await oidcService.PopulateAccountClaims(ctx);
        }

        private static SecurityKey GetIssuerSigningKey(GovUkOidcConfiguration config, IAzureIdentityService azureIdentityService)
        {


            if (config.UseKeyVault())
            {
                return new KeyVaultSecurityKey(config.Oidc.KeyVaultIdentifier, azureIdentityService.AuthenticationCallback);
            }

            var unencodedKey = config.Oidc.PrivateKey!;
            var privateKeyBytes = Convert.FromBase64String(unencodedKey);

            var bytes = Encoding.ASCII.GetBytes(unencodedKey);

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            var key = new RsaSecurityKey(rsa);
            return key;
        }

    }
}
