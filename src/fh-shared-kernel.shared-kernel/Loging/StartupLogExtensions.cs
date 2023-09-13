using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace FamilyHubs.SharedKernel.Loging;

public static class StartupLogExtensions
{
    public static void SharedConfigureHost(this WebApplicationBuilder builder)
    {
        // ApplicationInsights
        builder.Host.UseSerilog((_, services, loggerConfiguration) =>
        {
            var logLevelString = builder.Configuration["LogLevel"];

            var parsed = Enum.TryParse<LogEventLevel>(logLevelString, out var logLevel);

            var blobStorrageConnectionString = builder.Configuration["BlobStorageConnectionString"];
            ArgumentNullException.ThrowIfNull(blobStorrageConnectionString);
            var storageFileName = builder.Configuration["BlobStorageFilename"];
            ArgumentNullException.ThrowIfNull(storageFileName);
            loggerConfiguration.WriteTo.AzureBlobStorage(connectionString: blobStorrageConnectionString, restrictedToMinimumLevel: parsed ? logLevel : LogEventLevel.Warning, storageFileName: storageFileName);

            loggerConfiguration.WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces,
                parsed ? logLevel : LogEventLevel.Warning);

            loggerConfiguration.WriteTo.Console(
                parsed ? logLevel : LogEventLevel.Warning);
        });

        builder.Logging.AddAzureWebAppDiagnostics();
    }
}
