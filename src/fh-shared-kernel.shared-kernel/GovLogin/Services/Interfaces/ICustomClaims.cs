using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.GovLogin.Services.Interfaces
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}
