using Azure;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace FamilyHubs.SharedKernel.Identity.Authorisation.FamilyHubs
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
            var json = await CallClaimsApi(tokenValidatedContext);
            var claims = ExtractClaimsFromResponse(json);

            return claims;
        }

        private async Task<string> CallClaimsApi(TokenValidatedContext tokenValidatedContext)
        {
            var email = tokenValidatedContext?.Principal?.Identities.First().Claims
                    .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_httpClient.BaseAddress + $"api/AccountClaims/GetAccountClaimsByEmail?email={email}"),
            };

            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return "[]";
            }
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            return json;
        }

        private IEnumerable<Claim> ExtractClaimsFromResponse(string json)
        {
            var customClaims = JsonSerializer.Deserialize<List<AccountClaim>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var claims = customClaims.ConvertToSecurityClaim();

            claims.Add(new Claim(FamilyHubsClaimTypes.LoginTime, DateTime.UtcNow.Ticks.ToString()));

            return claims.AsEnumerable();
        }
    }
}
