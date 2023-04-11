using FamilyHubs.SharedKernel.Razor.Error;
using FamilyHubs.SharedKernel.Razor.Security;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Use all Family Hubs components.
    /// </summary>
    public static WebApplication UseFamilyHubs(this WebApplication app)
    {
        app.UseAppSecurityHeaders()
            .UseErrorHandling();

        return app;
    }
}