using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity.Authorisation.FamilyHubs;
using FamilyHubs.SharedKernel.Identity.Exceptions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace FamilyHubs.SharedKernel.Identity
{
    public interface ITermsAndConditionsService
    {
        public Task AcceptTermsAndConditions();
    }

    public class StubTermsAndConditionsService: ITermsAndConditionsService
    {
        public Task AcceptTermsAndConditions()
        {
            return Task.CompletedTask;
        }
    }

    public class TermsAndConditionsService : ITermsAndConditionsService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GovUkOidcConfiguration _configuration;

        public TermsAndConditionsService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, GovUkOidcConfiguration configuration)
        {
            _httpClient = httpClientFactory?.CreateClient(nameof(FamilyHubsClaims))!;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task AcceptTermsAndConditions()
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            var claim = CreateAccountClaim(httpContext);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_httpClient.BaseAddress + $"api/AccountClaims/AddClaim"),
                Content = new StringContent(JsonConvert.SerializeObject(claim), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_httpContextAccessor.HttpContext!.GetBearerToken()}");

            using var response = await _httpClient.SendAsync(request);

            if(!response.IsSuccessStatusCode) 
            {
                throw new ClaimsException($"Failed to add TermsAndCondtionsAccepted claim, returned with statusCode:{response.StatusCode}");
            }

            httpContext.RefreshClaims();
        }

        private AccountClaim CreateAccountClaim(HttpContext httpContext)
        {
            var user = httpContext.GetFamilyHubsUser();

            if(user == null)
                throw new ClaimsException($"Failed to add TermsAndCondtionsAccepted claim, could not extract FamilyHubsUser from identity context");

            var accountId = long.Parse(user.AccountId);

            var claim = new AccountClaim
            {
                AccountId = accountId,
                Name = $"{FamilyHubsClaimTypes.TermsAndConditionsAccepted}-{_configuration.AppHost}",
                Value = DateTime.UtcNow.ToString()
            };

            return claim;
        }
    }
}
