using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using FamilyHubs.SharedKernel.EntityFramework;

namespace FamilyHubs.SharedKernel.DataProtection;

//https://stackoverflow.com/questions/72010688/asp-net-core-3-1-unable-to-unprotect-the-message-state-running-in-debugger
/// <remarks>
/// https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-7.0
/// https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-7.0&tabs=visual-studio#entity-framework-core
/// </remarks>
public static class DataProtectionExtension
{
    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        // reuse same config?
        // put both in own section??
        string? keyVaultIdentifier = configuration["Crypto:KeyVaultIdentifier"];
        if (string.IsNullOrEmpty(keyVaultIdentifier))
        {
            //todo: use config exception
            throw new ArgumentException("Crypto:KeyVaultIdentifier value missing.");
        }

        // Add a DbContext to store your Database Keys
        services.AddDbContext<DataProtectionKeysContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DataProtectionKeysConnection"),
                ob => ob.MigrationsAssembly(typeof(DataProtectionKeysContext).Assembly.ToString())));

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataProtectionKeysContext>();
            MigrationUtility.ApplyMigrations(dbContext);
        }

        services.AddDataProtection()
            .SetApplicationName(appName)
            .PersistKeysToDbContext<DataProtectionKeysContext>()
            .ProtectKeysWithAzureKeyVault(new Uri(keyVaultIdentifier), new DefaultAzureCredential());
    }
}