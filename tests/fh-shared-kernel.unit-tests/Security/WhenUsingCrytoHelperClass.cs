using FamilyHubs.SharedKernel.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;

namespace FamilyHubs.SharedKernel.UnitTests.Security;

public class WhenUsingCrytoHelperClass
{
    /*
    [Fact]
    public void ThenEncryptStringAndThenDecryptItBack()
    {
        //Arrange
        (string publicKey, string privateKey) = GenerateKeys();

        Mock<IConfiguration> configuration = new Mock<IConfiguration>();
        configuration.Setup(x => x["Crypto:PublicKey"]).Returns(publicKey);
        configuration.Setup(x => x["Crypto:PrivateKey"]).Returns(privateKey);
        string expected = "Hello, RSA encryption!";
        ICrypto crypto = new Crypto(configuration.Object);

        //Act
        string encryptedData = crypto.EncryptData(expected);
        string decryptedData = crypto.DecryptData(encryptedData);

        //Assert
        expected.Should().Be(decryptedData);
        expected.Should().NotBe(encryptedData);
        encryptedData.Length.Should().BeGreaterThan(expected.Length);
    }
    */

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
