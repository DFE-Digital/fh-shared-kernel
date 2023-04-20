namespace FamilyHubs.SharedKernel.GovLogin.Services.Interfaces
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }
}
