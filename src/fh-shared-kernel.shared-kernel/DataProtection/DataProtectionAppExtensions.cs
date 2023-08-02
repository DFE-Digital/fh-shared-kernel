using FamilyHubs.SharedKernel.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.SharedKernel.DataProtection;

public static class DataProtectionAppExtensions
{
    public static WebApplication UseFamilyHubsDataProtection(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DataProtectionKeysContext>();
        MigrationUtility.ApplyMigrations(dbContext);

        return app;
    }
}