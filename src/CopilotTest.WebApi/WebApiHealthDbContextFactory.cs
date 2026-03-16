using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.WebApi;

internal class WebApiHealthDbContextFactory : IDesignTimeDbContextFactory<WebApiHealthDbContext>
{
    public WebApiHealthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WebApiHealthDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString, options =>
            options.MigrationsHistoryTable("__EFMigrationsHistory", "Health"));

        return new WebApiHealthDbContext(optionsBuilder.Options);
    }
}
