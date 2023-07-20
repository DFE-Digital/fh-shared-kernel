using System.Text.Json.Serialization;

namespace FamilyHubs.SharedKernel.Exceptions
{
    public class ApiExceptionResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public int StatusCode { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public IEnumerable<object> Errors { get; set; } = new List<object>();

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; } = string.Empty;

    }
}
