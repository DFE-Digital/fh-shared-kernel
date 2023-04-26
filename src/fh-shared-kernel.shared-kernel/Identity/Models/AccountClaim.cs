namespace FamilyHubs.SharedKernel.Identity.Models
{
    public class AccountClaim
    {
        public required string AccountId { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
