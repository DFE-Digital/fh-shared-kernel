using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Extensions;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FamilyHubs.SharedKernel.Razor.Cookies;

//todo: global one to add everything? then remove addfamilyhubsui?
public static class IServiceCollectionExtensions
{
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