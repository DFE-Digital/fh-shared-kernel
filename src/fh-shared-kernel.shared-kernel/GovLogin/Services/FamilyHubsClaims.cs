using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.GovLogin.Models;
using FamilyHubs.SharedKernel.GovLogin.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using System.Text.Json;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public class FamilyHubsClaims : ICustomClaims
    {
        private HttpClient _httpClient;
        public FamilyHubsClaims(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory?.CreateClient(nameof(FamilyHubsClaims))!;
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


}
