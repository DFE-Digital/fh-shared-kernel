using Azure.Identity;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Authentication.Gov;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace FamilyHubs.SharedKernel.Identity.SigningKey
{
    public class KeyVaultSigningKeyProvider : ISigningKeyProvider
    {
        private GovUkOidcConfiguration _configuration;
        private IAzureIdentityService _azureIdentityService;

        public KeyVaultSigningKeyProvider(GovUkOidcConfiguration govUkOidcConfiguration, IAzureIdentityService azureIdentityService)
        {
            _configuration = govUkOidcConfiguration;
            _azureIdentityService = azureIdentityService;
        }

        public SecurityKey GetBearerTokenSigningKey()
        {

            return new KeyVaultSecurityKey(_configuration.Oidc.KeyVaultIdentifier, _azureIdentityService.AuthenticationCallback);
        }
    }
}
