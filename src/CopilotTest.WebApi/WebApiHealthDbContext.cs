using Microsoft.EntityFrameworkCore;

namespace CopilotTest.WebApi;

internal class WebApiHealthDbContext : DbContext
{
    public WebApiHealthDbContext(DbContextOptions<WebApiHealthDbContext> options) : base(options)
    {
    }

    internal DbSet<Health> Health { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure schema for this bounded context
        modelBuilder.HasDefaultSchema("Health");

        modelBuilder.Entity<Health>(entity =>
        {
            entity.ToTable("health");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
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
            b.MigrationsHistoryTable("__EFMigrationsHistory", "Health"));
    }
}
