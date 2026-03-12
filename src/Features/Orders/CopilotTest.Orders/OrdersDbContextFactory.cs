using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.Orders;

internal class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=copilottestdb;Username=copilottest;Password=copilottest");

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
