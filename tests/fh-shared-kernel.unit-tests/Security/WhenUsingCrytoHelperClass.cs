using FamilyHubs.SharedKernel.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;

namespace FamilyHubs.SharedKernel.UnitTests.Security;

public class WhenUsingCrytoHelperClass
{
    
    [Fact]
    public async Task ThenEncryptStringAndThenDecryptItBack()
    {
        //Arrange
        (string publicKey, string privateKey) = GenerateKeys();

        Mock<IKeyProvider> provider = new Mock<IKeyProvider>();
        provider.Setup(x => x.GetPublicKey()).ReturnsAsync(publicKey);
        provider.Setup(x => x.GetPrivateKey()).ReturnsAsync(privateKey);

        string expected = "Hello, RSA encryption!";
        ICrypto crypto = new Crypto(provider.Object);

        //Act
        string encryptedData = await crypto.EncryptData(expected);
        string decryptedData = await crypto.DecryptData(encryptedData);

        //Assert
        expected.Should().Be(decryptedData);
        expected.Should().NotBe(encryptedData);
        encryptedData.Length.Should().BeGreaterThan(expected.Length);
    }
    

    public (string publicKey, string privateKey) GenerateKeys()
    {
        using (var rsa = RSA.Create())
        {
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);
            return (publicKey, privateKey);
        }
    }
}
