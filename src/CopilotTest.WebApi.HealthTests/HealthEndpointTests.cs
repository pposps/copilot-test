using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CopilotTest.WebApi;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

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
            // Remove all DbContext-related service descriptors
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<WebApiHealthDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            // Add in-memory database for testing with application service provider
            services.AddDbContext<WebApiHealthDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("TestHealthDb")
                       .UseApplicationServiceProvider(sp);
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

