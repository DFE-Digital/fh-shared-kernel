using System.Text.Json.Serialization;
using FamilyHubs.SharedKernel.Services.Postcode.Model;

namespace FamilyHubs.SharedKernel.Services.PostcodesIo;

public sealed class PostcodesIoResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("result")]
    public PostcodeInfo PostcodeInfo { get; set; } = default!;
}
