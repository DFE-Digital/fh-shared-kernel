using System.Security.Cryptography;
using System.Text;

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

    private readonly IKeyProvider _keyProvider;

    public Crypto(IKeyProvider keyProvider)
    {
        _keyProvider = keyProvider;
    }

    public async Task<string> EncryptData(string data)
    {
        string publicKey = await _keyProvider.GetPublicKey();
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
        string privateKey = await _keyProvider.GetPrivateKey();
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
}
