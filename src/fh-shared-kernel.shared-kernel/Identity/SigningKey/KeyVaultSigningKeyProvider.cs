using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Authentication.Gov;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;

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

        public SecurityKey GetSecurityKey()
        {
            return new KeyVaultSecurityKey(_configuration.Oidc.KeyVaultIdentifier, _azureIdentityService.AuthenticationCallback);
        }
    }
}
