using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.Orders;

internal class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();

        // Use connection string from environment or default
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=copilottestdb;Username=copilottest";

        optionsBuilder.UseNpgsql(connectionString, options =>
            options.MigrationsHistoryTable("__EFMigrationsHistory", "Orders"));

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
