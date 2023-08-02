using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.SharedKernel.DataProtection;

//https://stackoverflow.com/questions/72010688/asp-net-core-3-1-unable-to-unprotect-the-message-state-running-in-debugger
/// <remarks>
/// https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-7.0
/// https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-7.0&tabs=visual-studio#entity-framework-core
/// </remarks>
public static class DataProtectionExtension
{
    public static void AddFamilyHubsDataProtection(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        // Add a DbContext to store your Database Keys
        services.AddDbContext<DataProtectionKeysContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SharedKernelConnection"),
                ob =>
                {
                    ob.MigrationsHistoryTable("SharedKernelMigrationsHistory", "dbo");
                    ob.MigrationsAssembly(typeof(DataProtectionKeysContext).Assembly.ToString());
                }));

        // reuse same config? have an interface that the config section implements, then have a config to give the section name?
        // put both in own section??
        // we need the kv id to be a valid uri, so we can't reuse this..
        string? keyVaultIdentifier = configuration["Crypto:KeyVaultIdentifier"];
        if (string.IsNullOrEmpty(keyVaultIdentifier))
        {
            //todo: use config exception
            throw new ArgumentException("Crypto:KeyVaultIdentifier value missing.");
        }

        string? tenantId = configuration.GetValue<string>("Crypto:tenantId");
        if (string.IsNullOrEmpty(tenantId))
        {
            throw new ArgumentException("tenantId value missing.");
        }
        string? clientId = configuration.GetValue<string>("Crypto:clientId");
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentException("clientId value missing.");
        }
        string? clientSecret = configuration.GetValue<string>("Crypto:clientSecret");
        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentException("clientSecret value missing.");
        }

        services.AddDataProtection()
            .SetApplicationName(appName)
            .PersistKeysToDbContext<DataProtectionKeysContext>()
            .ProtectKeysWithAzureKeyVault(new Uri(keyVaultIdentifier), new ClientSecretCredential(tenantId, clientId, clientSecret));
    }
}