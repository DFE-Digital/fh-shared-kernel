using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext);
    }

    public class CustomClaims : ICustomClaims
    {
        private HttpClient _httpClient;
        public CustomClaims(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory?.CreateClient(nameof(CustomClaims))!;
        }

        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var accountId = tokenValidatedContext?.Principal?.Identities.First().Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_httpClient.BaseAddress + $"api/Account/GetAccountClaims?accountId={accountId}"),
            };

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var customClaims = JsonSerializer.Deserialize<List<AccountClaim>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            
            var claims = new List<Claim>();

            foreach(var claim in customClaims!)
            {
                claims.Add(new Claim(claim.Name, claim.Value));
            }

            return claims.AsEnumerable();
        }
    }

    public class StubCustomClaims : ICustomClaims
    {
        private List<AccountClaim> _claims;

        public StubCustomClaims(GovUkOidcConfiguration govUkOidcConfiguration)
        {
            _claims = govUkOidcConfiguration.StubAuthentication.StubClaims;
        }

        public Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var claims = new List<Claim>();

            foreach (var claim in _claims)
            {
                claims.Add(new Claim(claim.Name, claim.Value));
            }

            return Task.FromResult(claims.AsEnumerable());
        }
    }
}
