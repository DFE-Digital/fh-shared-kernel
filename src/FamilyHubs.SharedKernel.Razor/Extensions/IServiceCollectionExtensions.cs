using FamilyHubs.SharedKernel.Razor.Cookies;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Family Hubs components.
    /// </summary>
    /// <param name="cookiePolicyContent">The name of the Razor view containing cookie policy content for the cookie page.</param>
    /// <returns></returns>
    public static IServiceCollection AddFamilyHubs(
        this IServiceCollection services,
        IConfiguration configuration,
        string cookiePolicyContent = "_CookiePolicy.cshtml")
    {
        services.AddCookiePage(configuration);

        return services;
    }
}