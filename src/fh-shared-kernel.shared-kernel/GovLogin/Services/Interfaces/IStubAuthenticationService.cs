using FamilyHubs.SharedKernel.GovLogin.Models;
using Microsoft.AspNetCore.Http;

namespace FamilyHubs.SharedKernel.GovLogin.Services.Interfaces
{
    public interface IStubAuthenticationService
    {
        void AddStubEmployerAuth(IResponseCookies cookies, StubAuthUserDetails model);
    }
}
