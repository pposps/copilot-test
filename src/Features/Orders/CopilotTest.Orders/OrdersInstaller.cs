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
        // Register DbContext
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Register services
        services.AddScoped<IOrders, Orders>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}
