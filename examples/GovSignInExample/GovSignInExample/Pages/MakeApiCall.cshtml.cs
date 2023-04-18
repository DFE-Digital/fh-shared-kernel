using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GovSignInExample.Pages
{
    //[Authorize] - Leave out authorize attribute so a unauthorised response and be returned from the endpoint
    public class MakeApiCallModel : PageModel
    {
        private HttpClient _httpClient;
        public List<KeyValuePair<string, string>>? RequestResponse { get; set; }
        public string StatusCode { get; set; } = string.Empty;

        public MakeApiCallModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory?.CreateClient("TestClient")!;
        }

        public async Task OnGet()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_httpClient.BaseAddress + "Secure/Test")
            };

            using var response = await _httpClient.SendAsync(request);

            StatusCode = $"{(int)response.StatusCode} {response.StatusCode.ToString()}";

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
                return;

            RequestResponse = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
