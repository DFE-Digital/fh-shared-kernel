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
    public static void AddFamilyHubsDataProtection(
        this IServiceCollection services,
        IConfiguration configuration,
        string appName)
    {
        var dpOptions = configuration.GetSection("DataProtection").Get<DataProtectionOptions>()
                        ?? throw new ArgumentException("DataProtection section missing from configuration.");

        dpOptions.Validate();

        switch (dpOptions.KeyPersistence)
        {
            case DataProtectionKeyPersistence.Disabled:
                return;
            case DataProtectionKeyPersistence.AzureBlobStorage:
                SetupDataProtectionWithPersistToBlobStorage(services, configuration, appName, dpOptions);
                break;
            case DataProtectionKeyPersistence.DatabaseTable:
                SetupDataProtectionWithPersistToDatabase(services, configuration, appName, dpOptions);
                break;
            default:
                //todo: better exception
                throw new ArgumentException(nameof(dpOptions.KeyPersistence));
        }
    }

    private static void SetupDataProtectionWithPersistToBlobStorage(
        IServiceCollection services,
        IConfiguration configuration,
        string appName,
        DataProtectionOptions dpOptions)
    {
        throw new NotImplementedException("Blob storage not implemented yet.");

        // looks like we should use a managed identity service principal for this
        // https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&bc=%2Fazure%2Fstorage%2Fblobs%2Fbreadcrumb%2Ftoc.json&tabs=command-line#authentication-in-server-environments

        //todo: move common out and only do persist?
        //services.AddDataProtection()
        //    .SetApplicationName(appName)
        //    .PersistKeysToAzureBlobStorage()
        //    .ProtectKeysWithAzureKeyVault(new Uri(dpOptions.KeyIdentifier!),
        //        new ClientSecretCredential(dpOptions.TenantId!, dpOptions.ClientId!, dpOptions.ClientSecret!));
    }

    private static void SetupDataProtectionWithPersistToDatabase(
        IServiceCollection services,
        IConfiguration configuration,
        string appName,
        DataProtectionOptions dpOptions)
    {
        //todo: put the MigrationsHistoryTable and the DataProtectionKeys table in a "sharedkernel" schema
        const string schemaName = "dbo";

        // Add a DbContext to store your Database Keys
        services.AddDbContext<DataProtectionKeysContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SharedKernelConnection"),
                ob =>
                {
                    ob.MigrationsHistoryTable("SharedKernelMigrationsHistory", schemaName)
                        .MigrationsAssembly(typeof(DataProtectionKeysContext).Assembly.ToString());
                }));

        services.AddDataProtection()
            .SetApplicationName(appName)
            .PersistKeysToDbContext<DataProtectionKeysContext>()
            .ProtectKeysWithAzureKeyVault(new Uri(dpOptions.KeyIdentifier!),
                new ClientSecretCredential(dpOptions.TenantId!, dpOptions.ClientId!, dpOptions.ClientSecret!));
    }
}