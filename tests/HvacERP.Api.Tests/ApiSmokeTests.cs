using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

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

        Assert.NotNull(
            body);

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

    private sealed record ApiInformation(
        string Application,
        string Status,
        string Version);
}

public sealed class ApiTestApplication
    : WebApplicationFactory<Program>
{
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
                            "Host=localhost;Port=5432;Database=hvac_erp;Username=hvac_erp;Password=hvac_erp_dev_password"
                    };

                configurationBuilder.AddInMemoryCollection(
                    settings);
            });
    }
}
