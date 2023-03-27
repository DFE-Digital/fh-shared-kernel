using System.ComponentModel.DataAnnotations;

namespace FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;

public class FamilyHubsUiOptions
{
    public const string FamilyHubsUi = "FamilyHubsUi";

    [Required]
    public string ServiceName { get; set; } = "";
    public Phase Phase { get; set; }
    public string FeedbackUrl { get; set; } = "";

    public AnalyticsOptions? Analytics { get; set; }

    public FooterOptions Footer { get; set; } = new();
}