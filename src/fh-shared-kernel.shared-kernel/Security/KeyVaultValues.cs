namespace FamilyHubs.SharedKernel.Security;

public record KeyVaultValues
{
    public required string KeyVaultIdentifier { get; set; }
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set;}
}
