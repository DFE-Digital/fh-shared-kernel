using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace FamilyHubs.SharedKernel.Razor.Error;

public static class WebApplicationExtensions
{
    public static WebApplication UseErrorHandling(this WebApplication app, bool forceExceptionHandler = false)
    {
        if (forceExceptionHandler || !app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error/Index");
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        return app;
    }
}