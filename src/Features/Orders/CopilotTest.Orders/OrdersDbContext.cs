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

        // Configure schema for this bounded context
        modelBuilder.HasDefaultSchema("Orders");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerName).HasColumnName("customer_name").IsRequired().HasMaxLength(255);
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2);
            entity.Property(e => e.OrderDate).HasColumnName("order_date").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Isolate migration history per feature
        if (!optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseNpgsql(b =>
            b.MigrationsHistoryTable("__EFMigrationsHistory", "Orders"));
    }
}
