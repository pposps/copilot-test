using Microsoft.EntityFrameworkCore;

namespace CopilotTest.Orders;

internal class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    internal DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema for Orders feature
        modelBuilder.HasDefaultSchema("Orders");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerName).HasColumnName("customer_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductName).HasColumnName("product_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnName("total_price").IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.OrderDate).HasColumnName("order_date").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
        });
    }
}
