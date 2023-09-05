namespace FamilyHubs.SharedKernel.Identity.Models;

public class FamilyHubsUser
{
    public string Role { get; set; } = string.Empty;
    public string OrganisationId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? ClaimsValidTillTime { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool TermsAndConditionsAccepted { get; set; }
}