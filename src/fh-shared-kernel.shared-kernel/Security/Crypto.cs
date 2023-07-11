using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace FamilyHubs.SharedKernel.Security;

public class Crypto
{
    public string _publicKey {  get; set; }
    public string _privateKey { get; set; }

    public Crypto(IConfiguration configuration) 
    { 
        _publicKey = configuration["Crypto:PublicKey"] ?? string.Empty;
        _privateKey = configuration["Crypto:PublicKey"] ?? string.Empty;
    }

    public string EncryptData(string data)
    {
        if (string.IsNullOrEmpty(_publicKey) || string.IsNullOrEmpty(_privateKey))
        {
            throw new ArgumentException("Public / Private keys have not been found in the configuration");
        }

        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(_publicKey);
            byte[] encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
            string encryptedData = Convert.ToBase64String(encryptedBytes);
            return encryptedData;
        }
    }

    public string DecryptData(string encryptedData)
    {
        if (string.IsNullOrEmpty(_publicKey) || string.IsNullOrEmpty(_privateKey))
        {
            throw new ArgumentException("Public / Private keys have not been found in the configuration");
        }

        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(_privateKey);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            string decryptedData = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedData;
        }
    }
}
