using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace FamilyHubs.SharedKernel.Identity.SigningKey
{
    /// <summary>
    /// Provides the signing key from local configuration (appsettings.json - GovUkOidcConfiguration.Oidc.PrivateKey)
    /// </summary>
    public class LocalSigningKeyProvider : ISigningKeyProvider
    {
        private byte[] _bytes;

        public LocalSigningKeyProvider(GovUkOidcConfiguration govUkOidcConfiguration)
        {
            var unencodedKey = govUkOidcConfiguration.Oidc.PrivateKey!;
            _bytes = Convert.FromBase64String(unencodedKey);
        }

        public SecurityKey GetSecurityKey()
        {
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(_bytes, out _);
            var key = new RsaSecurityKey(rsa);
            return key;
        }
    }
}
