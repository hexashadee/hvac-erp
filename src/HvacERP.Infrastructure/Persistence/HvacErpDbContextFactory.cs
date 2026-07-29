using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HvacERP.Infrastructure.Persistence;

public sealed class HvacErpDbContextFactory
    : IDesignTimeDbContextFactory<HvacErpDbContext>
{
    public HvacErpDbContext CreateDbContext(
        string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(
                "HVAC_ERP_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=hvac_erp;Username=postgres;Password=postgres";

        DbContextOptionsBuilder<HvacErpDbContext> optionsBuilder =
            new();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(
                    typeof(HvacErpDbContext).Assembly.FullName);

                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5);
            });

        return new HvacErpDbContext(
            optionsBuilder.Options);
    }
}
