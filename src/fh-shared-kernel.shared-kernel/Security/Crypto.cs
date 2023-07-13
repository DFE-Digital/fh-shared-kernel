using Azure.Core;
using Azure.Identity;
using Azure;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Azure.Security.KeyVault.Secrets;

namespace FamilyHubs.SharedKernel.Security;

public enum CryptoKey
{
    PublicKey,
    PrivateKey
}

public interface ICrypto
{
    Task<string> EncryptData(string data);
    Task<string> DecryptData(string encryptedData);
}

public class Crypto : ICrypto
{
    
    private readonly IConfiguration _configuration;
    public Crypto(IConfiguration configuration) 
    {
        _configuration = configuration;
    }

    public async Task<string> EncryptData(string data)
    {
        string publicKey = await GetKey(CryptoKey.PublicKey);
        if (string.IsNullOrEmpty(publicKey))
        {
            throw new ArgumentException("Private key has not been found.");
        }

        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(publicKey);
            byte[] encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
            string encryptedData = Convert.ToBase64String(encryptedBytes);
            return encryptedData;
        }
    }

    public async Task<string> DecryptData(string encryptedData)
    {
        string privateKey = await GetKey(CryptoKey.PrivateKey);
        if (string.IsNullOrEmpty(privateKey))
        {
            throw new ArgumentException("Private key has not been found.");
        }

        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(privateKey);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            string decryptedData = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedData;
        }
    }

    private async Task<string> GetKey(CryptoKey cryptoKey)
    {
        string keyValue = string.Empty;
        bool useKeyVault = _configuration.GetValue<bool>("Crypto:UseKeyVault");
        if (useKeyVault)
        {
            string publicKeySecretName = _configuration.GetValue<string>("Crypto:PublicKeySecretName") ?? throw new ArgumentException("PublicKeySecretName value missing.");
            string privateKeySecretName = _configuration.GetValue<string>("Crypto:PrivateKeySecretName") ?? throw new ArgumentException("PrivateKeySecretName value missing.");
            string keyVaultIdentifier = _configuration.GetValue<string>("Crypto:KeyVaultIdentifier") ?? throw new ArgumentException("KeyVaultIdentifier value missing."); 
            string teanantId = _configuration.GetValue<string>("Crypto:teanantId") ?? throw new ArgumentException("teanantId value missing.");
            string clientId = _configuration.GetValue<string>("Crypto:clientId") ?? throw new ArgumentException("clientId value missing.");
            string clientSecret = _configuration.GetValue<string>("Crypto:clientSecret") ?? throw new ArgumentException("clientSecret value missing.");

            if (cryptoKey == CryptoKey.PublicKey)
            {
                keyValue = await GetKeyValue(keyVaultIdentifier, publicKeySecretName, teanantId, clientId, clientSecret);
            }
            else
            {
                keyValue = await GetKeyValue(keyVaultIdentifier, privateKeySecretName, teanantId, clientId, clientSecret);
            }

        }
        else
        {
            if (cryptoKey == CryptoKey.PublicKey)
            {
                keyValue = _configuration.GetValue<string>("Crypto:PublicKey") ?? string.Empty;
            }
            else
            {
                keyValue = _configuration.GetValue<string>("Crypto:PrivateKey") ?? string.Empty;
            }
            
        }

        if (string.IsNullOrEmpty(keyValue))
        {
            throw new ArgumentException("Crypto Key has not been found.");
        }

        return keyValue;
    }

    private async Task<string> GetKeyValue(string keyVaultName, string keyName, string teanantId, string clientId, string clientSecret)
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
        if (!string.IsNullOrEmpty(teanantId) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            tokenCredential = new ClientSecretCredential(teanantId, clientId, clientSecret);
        }

        var client = new SecretClient(new Uri(kvUri), tokenCredential, options);

        Response<KeyVaultSecret> secret = await client.GetSecretAsync(keyName);

        return secret.Value.Value;
    }
}
