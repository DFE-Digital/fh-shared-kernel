using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Exceptions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using System.Text.Json;

namespace FamilyHubs.SharedKernel.Identity.Authorisation.FamilyHubs
{
    public class FamilyHubsClaims : ICustomClaims
    {
        private HttpClient _httpClient;
        private readonly int _claimsRefreshTimerMinutes;
        public FamilyHubsClaims(IHttpClientFactory httpClientFactory, GovUkOidcConfiguration govUkOidcConfiguration)
        {
            _httpClient = httpClientFactory?.CreateClient(nameof(FamilyHubsClaims))!;
            _claimsRefreshTimerMinutes = govUkOidcConfiguration.ClaimsRefreshTimerMinutes;
        }

        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var email = tokenValidatedContext?.Principal?.Identities.First().Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;

            if (string.IsNullOrEmpty(email))
                throw new OneLoginException("Invalid TokenValidatedContext returned from OneLogin: Email claim not present");

            var json = await CallClaimsApi(email);
            var claims = ExtractClaimsFromResponse(json);

            return claims;
        }

        public async Task<IEnumerable<Claim>> RefreshClaims(string email, List<Claim> currentClaims)
        {
            var json = await CallClaimsApi(email);
            var upToDateClaims = ExtractClaimsFromResponse(json);

            //  Get Updated Claims
            var refreshedClaims = upToDateClaims.ToList();

            //  Add any claims that dont come from our claims endpoint (some come from one login, these will already be in the current claims)
            foreach(var currentClaim in currentClaims)
            {
                if(!refreshedClaims.Where(x=>x.Type == currentClaim.Type).Any())
                {
                    refreshedClaims.Add(currentClaim);
                }
            }

            return refreshedClaims;
        }

        private async Task<string> CallClaimsApi(string email)
        {

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

            claims.Add(CreateRefreshClaim());

            return claims.AsEnumerable();
        }

        private Claim CreateRefreshClaim()
        {
            var refreshTime = DateTime.UtcNow.AddMinutes(_claimsRefreshTimerMinutes).Ticks.ToString();
            return new Claim(FamilyHubsClaimTypes.ClaimsValidTillTime, refreshTime);
        }
    }
}
