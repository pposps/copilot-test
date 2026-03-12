using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CopilotTest.WebApi;

namespace CopilotTest.WebApi.HealthTests;

public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthyFromDatabase()
    {
        // Arrange
        await _factory.SeedDatabaseAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        // The response is JSON, so it includes quotes around the string
        Assert.Equal("\"healthy\"", content);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsUnknownWhenDatabaseIsEmpty()
    {
        // Arrange - don't seed the database
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("\"unknown\"", content);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all EF Core service descriptors to completely clear out PostgreSQL provider
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.Namespace != null &&
                       (d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore") ||
                        d.ServiceType == typeof(WebApiHealthDbContext) ||
                        d.ServiceType == typeof(DbContextOptions<WebApiHealthDbContext>)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<WebApiHealthDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestHealthDb");
            });
        });
    }

    public async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WebApiHealthDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (!await db.Health.AnyAsync())
        {
            db.Health.Add(new Health { Status = "healthy" });
            await db.SaveChangesAsync();
        }
    }
}

