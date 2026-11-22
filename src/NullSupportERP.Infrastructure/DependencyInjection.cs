using Amazon.RDS.Util;
using NullSupportERP.Application.Dispatch.Queries.GetSchedules;
using NullSupportERP.Infrastructure.Dispatch.Queries;
using NullSupportERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace NullSupportERP.Infrastructure;

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

        bool useAwsIamAuthentication =
            string.Equals(
                Environment.GetEnvironmentVariable(
                    "AWS_RDS_IAM_AUTH"),
                "true",
                StringComparison.OrdinalIgnoreCase);

        if (useAwsIamAuthentication)
        {
            NpgsqlConnectionStringBuilder connectionBuilder =
                new(connectionString);

            connectionBuilder.SslMode = SslMode.Require;
            connectionBuilder.Password = null;

            NpgsqlDataSourceBuilder dataSourceBuilder =
                new(connectionBuilder.ConnectionString);

            dataSourceBuilder.UsePeriodicPasswordProvider(
                (settings, cancellationToken) =>
                {
                    string authenticationToken =
                        RDSAuthTokenGenerator.GenerateAuthToken(
                            settings.Host,
                            settings.Port,
                            settings.Username);

                    return ValueTask.FromResult(
                        authenticationToken);
                },
                successRefreshInterval:
                    TimeSpan.FromMinutes(10),
                failureRefreshInterval:
                    TimeSpan.FromSeconds(30));

            NpgsqlDataSource dataSource =
                dataSourceBuilder.Build();

            services.AddSingleton(dataSource);

            services.AddDbContext<NullSupportErpDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        dataSource,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(
                                typeof(NullSupportErpDbContext)
                                    .Assembly
                                    .FullName);

                            npgsqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5);
                        });
                });
        }
        else
        {
            services.AddDbContext<NullSupportErpDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        connectionString,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(
                                typeof(NullSupportErpDbContext)
                                    .Assembly
                                    .FullName);

                            npgsqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5);
                        });
                });
        }

        services.AddScoped<
            IDispatchScheduleQueries,
            DispatchScheduleQueries>();

        return services;
    }
}
