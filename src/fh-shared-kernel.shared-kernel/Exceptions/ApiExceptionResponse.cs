using System.Text.Json.Serialization;

namespace FamilyHubs.SharedKernel.Exceptions
{
    public class ApiExceptionResponse : ApiExceptionResponse<object>
    {

    }

    public class ApiExceptionResponse<T>
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public int StatusCode { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public IEnumerable<T> Errors { get; set; } = new List<T>();

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; } = string.Empty;

    }
}
