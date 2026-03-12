using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.Orders;

internal class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();

        // Use a default connection string for migrations
        // This will be replaced at runtime by the actual connection string
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=copilottestdb;Username=copilottest;Password=copilottest",
            b => b.MigrationsHistoryTable("__EFMigrationsHistory", "orders"));

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
