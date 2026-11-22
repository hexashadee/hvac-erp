using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NullSupportERP.Infrastructure.Persistence;

public sealed class NullSupportErpDbContextFactory
    : IDesignTimeDbContextFactory<NullSupportErpDbContext>
{
    public NullSupportErpDbContext CreateDbContext(
        string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(
                "NULLSUPPORT_ERP_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=nullsupport_erp;Username=postgres;Password=postgres";

        DbContextOptionsBuilder<NullSupportErpDbContext> optionsBuilder =
            new();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(
                    typeof(NullSupportErpDbContext).Assembly.FullName);

                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5);
            });

        return new NullSupportErpDbContext(
            optionsBuilder.Options);
    }
}
