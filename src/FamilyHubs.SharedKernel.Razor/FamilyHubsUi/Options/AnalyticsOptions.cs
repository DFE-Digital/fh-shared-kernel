
namespace FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;

public class AnalyticsOptions
{
    public string CookieName { get; set; } = "";

    /// <summary>
    /// If cookie policy changes and/or the user preferences object format needs to change,
    /// bump this version up. The user will then be shown the banner again to consent to the new policy.
    /// </summary>
    public int CookieVersion { get; set; } = 1;

    public string MeasurementId { get; set; } = "";
    public string ContainerId { get; set; } = "";

    public string CookiePageUrl { get; set; } = "/cookies";
}