
namespace FamilyHubs.SharedKernel.DataProtection;

internal class DataProtectionOptions
{
    public string? KeyIdentifier { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    public void Validate()
    {
        //todo: bring in config exception
        if (string.IsNullOrEmpty(KeyIdentifier))
        {
            throw new ArgumentException("DataProtection:KeyIdentifier config missing");
        }

        if (string.IsNullOrEmpty(TenantId))
        {
            throw new ArgumentException("DataProtection:TenantId config missing");
        }

        if (string.IsNullOrEmpty(ClientId))
        {
            throw new ArgumentException("DataProtection:ClientId config missing");
        }

        if (string.IsNullOrEmpty(ClientSecret))
        {
            throw new ArgumentException("DataProtection:ClientSecret config missing");
        }
    }
}