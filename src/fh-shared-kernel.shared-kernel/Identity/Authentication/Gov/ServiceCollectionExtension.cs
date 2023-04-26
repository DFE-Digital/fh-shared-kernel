﻿using FamilyHubs.SharedKernel.GovLogin.AppStart;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Authorisation;
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
                throw new Exception($"CookieName is not configured in {nameof(GovUkOidcConfiguration)} section of appsettings");

            services
                .AddAuthentication(options => ConfigureAuthenticationOptions(options))
                .AddOpenIdConnect(options => ConfigureOpenIdConnect(options, govUkConfiguration, govUkConfiguration.CookieName))
                .AddAuthenticationCookie(govUkConfiguration.CookieName, govUkConfiguration);

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IOidcService, IAzureIdentityService, ICustomClaims, GovUkOidcConfiguration>(
                    (options, oidcService, azureIdentityService, customClaims, config) =>
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
                    });
        }

        private static void ConfigureAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        }

        private static void ConfigureOpenIdConnect(OpenIdConnectOptions options, GovUkOidcConfiguration govUkConfiguration, string authenticationCookieName)
        {
            options.ClientId = govUkConfiguration.Oidc.ClientId;
            options.MetadataAddress = $"{govUkConfiguration.Oidc.BaseUrl}/.well-known/openid-configuration";
            options.ResponseType = "code";
            options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
            options.SignedOutRedirectUri = "/Account/logout-callback";
            options.SignedOutCallbackPath = "/Account/logout-callback";
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
                c.Response.Redirect(govUkConfiguration.Urls.SignedOutRedirect);
                c.HandleResponse();
                return Task.CompletedTask;
            };
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