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
    Task<string> GetDbEncryptionKey();
    Task<string> GetDbEncryptionIVKey();
}

public class KeyProvider : IKeyProvider
{
    private readonly IConfiguration _configuration;
    private string? _publicKey;
    private string? _privateKey;
    private string? _dbEncryptionKey;
    private string? _dbEncryptionIVKey;

    public KeyProvider(IConfiguration configuration)
    {
        _configuration = configuration;

    }

    public async Task<string> GetDbEncryptionKey()
    {
        if (!string.IsNullOrEmpty(_dbEncryptionKey))
        {
            return _dbEncryptionKey;
        }

        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault)
        {
            _dbEncryptionKey = _configuration.GetValue<string>("Crypto:DbEncryptionKey") ?? throw new ArgumentException("DbEncryptionKey value missing.");
            return _dbEncryptionKey;
        }

        string? publicSecretName = _configuration.GetValue<string>("Crypto:DbEncryptionKeySecretName");
        if (string.IsNullOrEmpty(publicSecretName))
        {
            throw new ArgumentException("DbEncryptionKeySecretName value missing.");
        }

        KeyVaultValues kvv = GetKeyVaultValues();

        _dbEncryptionKey = await GetKeyValue(kvv.KeyVaultIdentifier, publicSecretName, kvv.TenantId, kvv.ClientId, kvv.ClientSecret);
        return _dbEncryptionKey;
    }

    public async Task<string> GetDbEncryptionIVKey()
    {
        if (!string.IsNullOrEmpty(_dbEncryptionIVKey))
        {
            return _dbEncryptionIVKey;
        }

        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault)
        {
            _dbEncryptionIVKey = _configuration.GetValue<string>("Crypto:DbEncryptionIVKey") ?? throw new ArgumentException("DbEncryptionIVKey value missing.");
            return _dbEncryptionIVKey;
        }

        string? publicSecretName = _configuration.GetValue<string>("Crypto:DbEncryptionIVKeySecretName");
        if (string.IsNullOrEmpty(publicSecretName))
        {
            throw new ArgumentException("DbEncryptionIVKeySecretName value missing.");
        }

        KeyVaultValues kvv = GetKeyVaultValues();

        _dbEncryptionIVKey = await GetKeyValue(kvv.KeyVaultIdentifier, publicSecretName, kvv.TenantId, kvv.ClientId, kvv.ClientSecret);
        return _dbEncryptionIVKey;
    }

    public async Task<string> GetPublicKey()
    {
        if (!string.IsNullOrEmpty(_publicKey)) 
        { 
            return _publicKey;
        }

        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault) 
        {
            _publicKey = _configuration.GetValue<string>("Crypto:PublicKey") ?? throw new ArgumentException("PublicKey value missing.");
            return _publicKey;
        }

        string? publicKeySecretName = _configuration.GetValue<string>("Crypto:PublicKeySecretName");
        if (string.IsNullOrEmpty(publicKeySecretName))
        {
            throw new ArgumentException("PublicKeySecretName value missing.");
        }

        KeyVaultValues kvv = GetKeyVaultValues();

        _publicKey = await GetKeyValue(kvv.KeyVaultIdentifier, publicKeySecretName, kvv.TenantId, kvv.ClientId, kvv.ClientSecret);
        return _publicKey;
    }

    public async Task<string> GetPrivateKey()
    {
        if (!string.IsNullOrEmpty(_privateKey))
        {
            return _privateKey;
        }

        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (!useKeyVault)
        {
            _privateKey = _configuration.GetValue<string>("Crypto:PrivateKey") ?? throw new ArgumentException("PrivateKey value missing.");
            return _privateKey;
        }

        string? privateKeySecretName = _configuration.GetValue<string>("Crypto:PrivateKeySecretName");
        if (string.IsNullOrEmpty(privateKeySecretName))
        {
            throw new ArgumentException("PrivateKeySecretName value missing.");
        }
        
        KeyVaultValues kvv = GetKeyVaultValues();

        _privateKey = await GetKeyValue(kvv.KeyVaultIdentifier, privateKeySecretName, kvv.TenantId, kvv.ClientId, kvv.ClientSecret);
        return _privateKey;
    }

    private KeyVaultValues GetKeyVaultValues()
    {
        string? keyVaultIdentifier = _configuration.GetValue<string>("Crypto:KeyVaultIdentifier");
        if (string.IsNullOrEmpty(keyVaultIdentifier))
        {
            throw new ArgumentException("KeyVaultIdentifier value missing.");
        }
        string? tenantId = _configuration.GetValue<string>("Crypto:tenantId");
        if (string.IsNullOrEmpty(tenantId))
        {
            throw new ArgumentException("tenantId value missing.");
        }
        string? clientId = _configuration.GetValue<string>("Crypto:clientId");
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentException("clientId value missing.");
        }
        string? clientSecret = _configuration.GetValue<string>("Crypto:clientSecret");
        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentException("clientSecret value missing.");
        }

        KeyVaultValues keyVaultValues = new KeyVaultValues
        {
            KeyVaultIdentifier = keyVaultIdentifier,
            TenantId = tenantId,
            ClientId = clientId,
            ClientSecret = clientSecret
        };

        return keyVaultValues;
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