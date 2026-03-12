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

        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerName).HasColumnName("customer_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2);
            entity.Property(e => e.OrderDate).HasColumnName("order_date").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
        });
    }
}
