using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Extensions;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FamilyHubs.SharedKernel.Razor.Cookies;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Cookie page component.
    /// </summary>
    /// <param name="cookiePolicyContent">The name of the Razor view containing cookie policy content for the cookie page.</param>
    /// <returns></returns>
    public static IServiceCollection AddCookiePage(
        this IServiceCollection services,
        IConfiguration configuration,
        string cookiePolicyContent = "_CookiePolicy.cshtml")
    {
        services.AddFamilyHubsUi(configuration);
        services.AddSingleton<ICookiePage>(serviceProvider =>
            new CookiePage(serviceProvider.GetRequiredService<IOptions<FamilyHubsUiOptions>>(), cookiePolicyContent));
        return services;
    }
}