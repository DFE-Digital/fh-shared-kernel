using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.SharedKernel.EntityFramework;

public static class MigrationUtility
{
    public static void ApplyMigrations<TContext>(TContext dbContext) where TContext : DbContext
    {
        dbContext.Database.Migrate();
    }
}