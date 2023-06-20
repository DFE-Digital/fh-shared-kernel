using System.Security.Claims;

namespace FamilyHubs.SharedKernel.Identity.Models
{
    public class StubClaimsCookie
    {
        public StubClaimsCookie()
        {

        }

        public StubClaimsCookie(List<Claim> claims)
        {
            Claims = new List<StubClaim>();

            foreach (var claim in claims)
            {
                if (!string.IsNullOrEmpty(claim.Type) && claim.Value != null)
                {
                    Claims.Add(new StubClaim { Name = claim.Type, Value = claim.Value });
                }
            }
        }

        public List<StubClaim> Claims { get; set; } = default!;
    }

    //  Need to use Stub claim as claim cannot be deserialized
    public class StubClaim
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
