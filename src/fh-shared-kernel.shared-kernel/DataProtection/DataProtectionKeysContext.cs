using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.SharedKernel.DataProtection;

class DataProtectionKeysContext : DbContext, IDataProtectionKeyContext
{
    public DataProtectionKeysContext()
    {
    }

    public DataProtectionKeysContext(DbContextOptions<DataProtectionKeysContext> options)
        : base(options)
    {
    }

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    //todo: put the MigrationsHistoryTable and the DataProtectionKeys table in a "sharedkernel" schema
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    const string schemaName = "sharedkernel";

    //    modelBuilder.Entity<DataProtectionKey>()
    //        .ToTable("DataProtectionKeys", schemaName);
    //}

    //todo: use IDesignTimeDbContextFactory instead for safety?
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=FamilyHubs.Referral.Database;Integrated Security=True;MultipleActiveResultSets=True;Pooling=False;TrustServerCertificate=True",
                x => x.MigrationsAssembly(typeof(DataProtectionKeysContext).Assembly.ToString()));
        }
    }
}