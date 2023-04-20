using Azure.Core;
using Azure.Identity;
using FamilyHubs.SharedKernel.GovLogin.Services.Interfaces;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    internal class AzureIdentityService : IAzureIdentityService
    {
        public async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            var chainedTokenCredential = new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential());

            var token = await chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] { "https://vault.azure.net/.default" }));

            return token.Token;
        }
    }
}
