using HvacERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HvacERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "A PostgreSQL connection string is required.",
                nameof(connectionString));
        }

        services.AddDbContext<HvacErpDbContext>(
            options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(
                            typeof(HvacErpDbContext).Assembly.FullName);

                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5);
                    });
            });

        return services;
    }
}
