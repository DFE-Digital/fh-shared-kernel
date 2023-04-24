using Microsoft.Extensions.Configuration;

namespace FamilyHubs.SharedKernel.GovLogin.Configuration
{
    public static class ConfigurationExtensions
    {
        public static GovUkOidcConfiguration GetGovUkOidcConfiguration(this IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(GovUkOidcConfiguration)).Get<GovUkOidcConfiguration>();
            if (config == null)
            {
                throw new ArgumentNullException(nameof(GovUkOidcConfiguration), "Could not get Section GovUkOidcConfiguration from configuration");
            }
            return config;
        }

        public static bool UseKeyVault(this GovUkOidcConfiguration configuration)
        {
            var keyVaultConfigIsNull = string.IsNullOrWhiteSpace(configuration.Oidc.KeyVaultIdentifier);
            var privateKeyConfigIsNull = string.IsNullOrWhiteSpace(configuration.Oidc.PrivateKey);

            if (keyVaultConfigIsNull && privateKeyConfigIsNull)
            {
                throw new ArgumentNullException("Either KeyVaultIdentifier or PrivateKey must be populated, both cannot be null");
            }

            if (!keyVaultConfigIsNull && !privateKeyConfigIsNull)
            {
                throw new ArgumentException("Both KeyVaultIdentifier or PrivateKey must not be populated, only one should be used");
            }

            if (!keyVaultConfigIsNull)
            {
                return true;
            }

            return false;
        }
    }
}
