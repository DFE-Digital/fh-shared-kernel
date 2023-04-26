﻿using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.SharedKernel.GovLogin.Configuration
{
    public class GovUkOidcConfiguration
    {
        public Oidc Oidc { get; set; } = default!;
        public Urls Urls { get; set; } = default!;
        public StubAuthentication StubAuthentication { get; set; } = new StubAuthentication();
        public int ExpiryInMinutes { get; set; } = 15;
        public string? IdamsApiBaseUrl { get; set; }
        public string? CookieName { get; set; }
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
        public bool UseStubClaims { get; set; } = false;
        public string StubDomain { get; set; } = "localhost";
        public List<StubUser>? StubUsers { get; set; }
        public string PrivateKey { get; set; } = "StubPrivateKey123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}