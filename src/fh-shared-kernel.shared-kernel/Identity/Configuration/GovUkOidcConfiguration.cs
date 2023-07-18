using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.SharedKernel.GovLogin.Configuration
{
    public class GovUkOidcConfiguration
    {
        public Oidc Oidc { get; set; } = default!;
        public Urls Urls { get; set; } = default!;
        public StubAuthentication StubAuthentication { get; set; } = new StubAuthentication();
        public int ExpiryInMinutes { get; set; } = 15;
        public int ClaimsRefreshTimerMinutes { get; set; } = 5;
        public string? IdamsApiBaseUrl { get; set; }
        public string? CookieName { get; set; }
        public string? AppHost { get; set; }
        public bool EnableDebugLogging { get; set; } = false;
        public string BearerTokenSigningKey { get; set; } = string.Empty;

    }

    public class Oidc
    {
        public string BaseUrl { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string? KeyVaultIdentifier { get; set; }
        public string? PrivateKey { get; set; }
        public bool TwoFactorEnabled { get; set; } = true;
    }

    public class Urls
    {
        public string SignedOutRedirect { get; set; } = string.Empty;
        public string AccountSuspendedRedirect { get; set; } = string.Empty;
        public string NoClaimsRedirect { get; set; } = string.Empty;
    }

    public class StubAuthentication
    {
        public bool UseStubAuthentication { get; set; } = false;
        public bool UseStubClaims { get; set; } = false;
        public List<AccountClaim>? StubClaims { get; set; }
        public List<StubUser>? StubUsers { get; set; }
    }
}
