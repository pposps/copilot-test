using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CopilotTest.Orders;

public static class OrdersInstaller
{
    public static IServiceCollection AddOrders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with PostgreSQL
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "orders")));

        // Register services
        services.AddScoped<IOrders, Orders>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }

    public static void ApplyOrdersMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        // Only run migrations if using a relational database (not in-memory)
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
    }
}
