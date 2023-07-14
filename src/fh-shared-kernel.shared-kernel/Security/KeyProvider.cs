using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.SharedKernel.Security;

public interface IKeyProvider
{
    Task<string> GetPublicKey();
    Task<string> GetPrivateKey();
}

public class KeyProvider : IKeyProvider
{
    private readonly IConfiguration _configuration;

    public KeyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetPublicKey()
    {
        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault) 
        {
            return _configuration.GetValue<string>("Crypto:PublicKey") ?? throw new ArgumentException("PublicKey value missing.");
        }

        string publicKeySecretName = _configuration.GetValue<string>("Crypto:PublicKeySecretName") ?? throw new ArgumentException("PublicKeySecretName value missing.");
        string keyVaultIdentifier = _configuration.GetValue<string>("Crypto:KeyVaultIdentifier") ?? throw new ArgumentException("KeyVaultIdentifier value missing.");
        string tenantId = _configuration.GetValue<string>("Crypto:tenantId") ?? throw new ArgumentException("teanantId value missing.");
        string clientId = _configuration.GetValue<string>("Crypto:clientId") ?? throw new ArgumentException("clientId value missing.");
        string clientSecret = _configuration.GetValue<string>("Crypto:clientSecret") ?? throw new ArgumentException("clientSecret value missing.");


        if (string.IsNullOrEmpty(publicKeySecretName) || string.IsNullOrEmpty(keyVaultIdentifier) ||
            string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentException("One or more key configuration values are missing.");
        }

        return await GetKeyValue(keyVaultIdentifier, publicKeySecretName, tenantId, clientId, clientSecret);
    }

    public async Task<string> GetPrivateKey()
    {
        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault)
        {
            return _configuration.GetValue<string>("Crypto:PrivateKey") ?? throw new ArgumentException("PrivateKey value missing.");
        }

        string privateKeySecretName = _configuration.GetValue<string>("Crypto:PrivateKeySecretName") ?? throw new ArgumentException("PrivateKeySecretName value missing.");
        string keyVaultIdentifier = _configuration.GetValue<string>("Crypto:KeyVaultIdentifier") ?? throw new ArgumentException("KeyVaultIdentifier value missing.");
        string tenantId = _configuration.GetValue<string>("Crypto:tenantId") ?? throw new ArgumentException("teanantId value missing.");
        string clientId = _configuration.GetValue<string>("Crypto:clientId") ?? throw new ArgumentException("clientId value missing.");
        string clientSecret = _configuration.GetValue<string>("Crypto:clientSecret") ?? throw new ArgumentException("clientSecret value missing.");

        if (string.IsNullOrEmpty(privateKeySecretName) || string.IsNullOrEmpty(keyVaultIdentifier) ||
            string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentException("One or more key configuration values are missing.");
        }

        return await GetKeyValue(keyVaultIdentifier, privateKeySecretName, tenantId, clientId, clientSecret);
    }

    private async Task<string> GetKeyValue(string keyVaultName, string keyName, string tenantId, string clientId, string clientSecret)
    {
        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        SecretClientOptions options = new SecretClientOptions()
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
             }
        };

        TokenCredential tokenCredential = new DefaultAzureCredential();
        if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            tokenCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        }

        var client = new SecretClient(new Uri(kvUri), tokenCredential, options);

        Response<KeyVaultSecret> secret = await client.GetSecretAsync(keyName);

        return secret.Value.Value;
    }
}