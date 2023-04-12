using FamilyHubs.SharedKernel.GovLogin.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public interface IOidcService
    {
        Task<Token?> GetToken(OpenIdConnectMessage openIdConnectMessage);
        Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext);
    }
}
