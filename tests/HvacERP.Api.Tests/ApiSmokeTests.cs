using System.Net;
using System.Net.Http.Json;
using HvacERP.Application.Dispatch.Queries.GetSchedules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HvacERP.Api.Tests;

public sealed class ApiSmokeTests
    : IClassFixture<ApiTestApplication>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(
        ApiTestApplication application)
    {
        _client =
            application.CreateClient();
    }

    [Fact]
    public async Task Root_ReturnsRunningApiInformation()
    {
        HttpResponseMessage response =
            await _client.GetAsync("/");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        ApiInformation? body =
            await response.Content
                .ReadFromJsonAsync<ApiInformation>();

        Assert.NotNull(body);

        Assert.Equal(
            "HVAC ERP API",
            body.Application);

        Assert.Equal(
            "running",
            body.Status);

        Assert.Equal(
            "0.1.0-beta",
            body.Version);
    }

    [Fact]
    public async Task GetDispatchSchedules_ReturnsSchedules()
    {
        HttpResponseMessage response =
            await _client.GetAsync(
                "/api/dispatch/schedules");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        DispatchScheduleDto[]? schedules =
            await response.Content
                .ReadFromJsonAsync<
                    DispatchScheduleDto[]>();

        Assert.NotNull(schedules);

        DispatchScheduleDto schedule =
            Assert.Single(schedules);

        Assert.Equal(
            ApiTestApplication.ScheduleId,
            schedule.Id);

        Assert.Equal(
            ApiTestApplication.WorkOrderId,
            schedule.WorkOrderId);

        Assert.Equal(
            "Scheduled",
            schedule.Status);

        Assert.Equal(
            ApiTestApplication.TechnicianUserId,
            schedule.PrimaryTechnicianUserId);

        DispatchScheduleTechnicianDto technician =
            Assert.Single(schedule.Technicians);

        Assert.Equal(
            ApiTestApplication.TechnicianUserId,
            technician.TechnicianUserId);

        Assert.Equal(
            "Primary",
            technician.Role);
    }

    private sealed record ApiInformation(
        string Application,
        string Status,
        string Version);
}

public sealed class ApiTestApplication
    : WebApplicationFactory<Program>
{
    public static readonly Guid ScheduleId =
        Guid.Parse(
            "10000000-0000-0000-0000-000000000001");

    public static readonly Guid WorkOrderId =
        Guid.Parse(
            "20000000-0000-0000-0000-000000000001");

    public static readonly Guid TechnicianUserId =
        Guid.Parse(
            "30000000-0000-0000-0000-000000000001");

    private static readonly Guid AssignmentId =
        Guid.Parse(
            "40000000-0000-0000-0000-000000000001");

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, configurationBuilder) =>
            {
                Dictionary<string, string?> settings =
                    new()
                    {
                        [
                            "ConnectionStrings:HvacErp"
                        ] =
                            "Host=localhost;Database=test;Username=test;Password=test"
                    };

                configurationBuilder
                    .AddInMemoryCollection(settings);
            });

        builder.ConfigureServices(
            services =>
            {
                services.RemoveAll<
                    IDispatchScheduleQueries>();

                services.AddSingleton<
                    IDispatchScheduleQueries>(
                    new StubDispatchScheduleQueries());
            });
    }

    private sealed class StubDispatchScheduleQueries
        : IDispatchScheduleQueries
    {
        public Task<
            IReadOnlyList<DispatchScheduleDto>>
            GetSchedulesAsync(
                CancellationToken cancellationToken =
                    default)
        {
            DateTimeOffset scheduledStartUtc =
                new(
                    2026,
                    8,
                    1,
                    13,
                    0,
                    0,
                    TimeSpan.Zero);

            DispatchScheduleTechnicianDto technician =
                new(
                    AssignmentId,
                    TechnicianUserId,
                    "Primary",
                    scheduledStartUtc.AddDays(-1));

            IReadOnlyList<DispatchScheduleDto> schedules =
            [
                new DispatchScheduleDto(
                    ScheduleId,
                    WorkOrderId,
                    scheduledStartUtc,
                    scheduledStartUtc.AddHours(2),
                    scheduledStartUtc,
                    scheduledStartUtc.AddMinutes(30),
                    "America/Detroit",
                    "Test dispatch appointment",
                    "Scheduled",
                    TechnicianUserId,
                    [technician])
            ];

            return Task.FromResult(schedules);
        }
    }
}
