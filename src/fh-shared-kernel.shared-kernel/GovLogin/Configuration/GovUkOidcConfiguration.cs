using FamilyHubs.SharedKernel.GovLogin.Models;

namespace FamilyHubs.SharedKernel.GovLogin.Configuration
{
    public class GovUkOidcConfiguration
    {
        public Oidc Oidc { get; set; } = default!;
        public Urls Urls { get; set; } = default!;
        public StubAuthentication StubAuthentication { get; set; } = new StubAuthentication();
    }

    public class Oidc
    {
        public string BaseUrl { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string? KeyVaultIdentifier { get; set; }
        public string? PrivateKey { get; set; }
    }

    public class Urls
    {
        public string SignedOutRedirect { get; set; } = default!;
        public string AccountSuspendedRedirect { get; set; } = default!;
    }

    public class StubAuthentication
    {
        public bool UseStubAuthentication { get; set; } = false;
        public string StubDomain { get; set; } = "localhost";
        public string AuthCookieName { get; set; } = "GovSignIn.StubAuthCookie";
        public GovUkUser GovUkUser { get; set; } = new GovUkUser { Email = "stub.user@demo.com", Sub = "stubCode" };
    }


}
