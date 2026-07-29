using HvacERP.Application.Dispatch.Queries.GetSchedules;
using HvacERP.Infrastructure;
using HvacERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

string? configuredConnectionString =
    builder.Configuration.GetConnectionString("HvacErp");

string connectionString =
    !string.IsNullOrWhiteSpace(configuredConnectionString)
        ? configuredConnectionString
        : Environment.GetEnvironmentVariable(
            "HVAC_ERP_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "The HVAC ERP database connection string is missing.");

builder.Services.AddInfrastructure(
    connectionString);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<HvacErpDbContext>(
        name: "postgresql");

builder.Services.AddOpenApi();

WebApplication app =
    builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet(
        "/",
        () => Results.Ok(
            new
            {
                application = "HVAC ERP API",
                status = "running",
                version = "0.1.0-beta"
            }))
    .WithName("GetApiInformation")
    .WithTags("System");

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter =
            async (context, report) =>
            {
                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        status =
                            report.Status.ToString(),
                        checks =
                            report.Entries.Select(
                                entry =>
                                    new
                                    {
                                        name =
                                            entry.Key,
                                        status =
                                            entry.Value.Status.ToString(),
                                        description =
                                            entry.Value.Description
                                    })
                    });
            }
    });

app.MapGet(
        "/api/system/database",
        async (
            HvacErpDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            bool canConnect =
                await dbContext.Database.CanConnectAsync(
                    cancellationToken);

            return canConnect
                ? Results.Ok(
                    new
                    {
                        database = "hvac_erp",
                        status = "connected"
                    })
                : Results.Problem(
                    title: "Database unavailable",
                    detail:
                        "The API could not connect to PostgreSQL.",
                    statusCode:
                        StatusCodes.Status503ServiceUnavailable);
        })
    .WithName("CheckDatabaseConnection")
    .WithTags("System");


app.MapGet(
        "/api/dispatch/schedules",
        async (
            IDispatchScheduleQueries queries,
            CancellationToken cancellationToken) =>
        {
            IReadOnlyList<DispatchScheduleDto> schedules =
                await queries.GetSchedulesAsync(
                    cancellationToken);

            return Results.Ok(schedules);
        })
    .WithName("GetDispatchSchedules")
    .WithTags("Dispatch");

app.Run();

public partial class Program;
