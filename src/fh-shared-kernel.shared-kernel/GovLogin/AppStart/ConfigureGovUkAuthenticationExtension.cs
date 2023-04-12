using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace FamilyHubs.SharedKernel.GovLogin.AppStart
{
    internal static class ConfigureGovUkAuthenticationExtension
    {
        internal static void ConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, string authenticationCookieName, string redirectUrl)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                }).AddOpenIdConnect(options =>
                {
                    var govUkConfiguration = configuration.GetGovUkOidcConfiguration();

                    options.ClientId = govUkConfiguration.Oidc.ClientId;
                    options.MetadataAddress = $"{govUkConfiguration.Oidc.BaseUrl}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = "/";
                    options.SignedOutCallbackPath = "/signed-out";
                    options.CallbackPath = "/Account/login-callback";
                    options.ResponseMode = string.Empty;

                    options.SaveTokens = true;

                    var scopes = "openid email phone".Split(' ');
                    options.Scope.Clear();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.Events.OnRemoteFailure = c =>
                    {
                        if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                        {
                            c.Response.Redirect("/");
                            c.HandleResponse();
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedOutCallbackRedirect = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName);
                        c.Response.Redirect(redirectUrl);
                        c.HandleResponse();
                        return Task.CompletedTask;
                    };


                }).AddAuthenticationCookie(authenticationCookieName);
            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IOidcService, IAzureIdentityService, ICustomClaims, GovUkOidcConfiguration>(
                    (options, oidcService, azureIdentityService, customClaims, config) =>
                    {
                        var govUkConfiguration = config;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            AuthenticationType = "private_key_jwt",
                            IssuerSigningKey = TempCodeGetSignInCred(),
                            //IssuerSigningKey = new KeyVaultSecurityKey(govUkConfiguration.KeyVaultIdentifier,
                            //    azureIdentityService.AuthenticationCallback),
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
                    });
        }

        private static RsaSecurityKey TempCodeGetSignInCred()
        {
            var unencodedKey = "";
            var privateKeyBytes = Convert.FromBase64String(unencodedKey);

            var bytes = Encoding.ASCII.GetBytes(unencodedKey);

            var rsa = RSA.Create();
            try
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            catch (Exception ex)
            {
                var foo = ex.Message;
            }
            var key = new RsaSecurityKey(rsa);
            return key;
        }
    }
}
