# Entity Framework Migrations Guide

This document explains how to work with Entity Framework migrations in the CopilotTest project, which uses a feature-specific migration strategy to maintain clear bounded contexts and prepare for potential microservice extraction.

## Architecture Overview

Each feature maintains its own:
- **DbContext**: Manages database operations for the feature's bounded context
- **Schema**: All tables are placed in a schema named after the feature (e.g., `Orders`, `WebApi`)
- **Migrations**: Independent migration history stored in the feature's `Migrations` folder

## Features with Migrations

### WebApi Feature
- **Schema**: `WebApi`
- **DbContext**: `WebApiHealthDbContext`
- **Tables**: `health`
- **Location**: `src/CopilotTest.WebApi/`

### Orders Feature
- **Schema**: `Orders`
- **DbContext**: `OrdersDbContext`
- **Tables**: `orders`
- **Location**: `src/Features/Orders/CopilotTest.Orders/`

## Working with Migrations

### Prerequisites

Install the Entity Framework Core tools globally:

```bash
dotnet tool install --global dotnet-ef
```

### Creating a New Migration

Navigate to the feature's project directory and create a migration:

```bash
# Example: Creating a migration for the Orders feature
cd src/Features/Orders/CopilotTest.Orders/

dotnet ef migrations add AddOrderStatusColumn \
    --context OrdersDbContext \
    --output-dir Migrations
```

**Important**: Always specify the `--context` parameter to ensure the migration is created for the correct DbContext.

### Viewing Migrations

List all migrations for a specific context:

```bash
# Example: List Orders migrations
cd src/Features/Orders/CopilotTest.Orders/
dotnet ef migrations list --context OrdersDbContext
```

### Applying Migrations

Apply migrations to update the database:

```bash
# Example: Apply Orders migrations
cd src/Features/Orders/CopilotTest.Orders/
dotnet ef database update --context OrdersDbContext
```

**Note**: In development, migrations can be applied automatically at startup if configured in the feature's Installer class.

### Removing a Migration

Remove the last migration (only if it hasn't been applied):

```bash
cd src/Features/Orders/CopilotTest.Orders/
dotnet ef migrations remove --context OrdersDbContext
```

### Generating SQL Scripts

Generate SQL scripts for migrations without applying them:

```bash
cd src/Features/Orders/CopilotTest.Orders/
dotnet ef migrations script --context OrdersDbContext

# Or generate SQL for a specific migration range
dotnet ef migrations script InitialCreate AddOrderStatusColumn \
    --context OrdersDbContext \
    --output migration_script.sql
```

## Creating a New Feature with Migrations

When creating a new feature that requires database persistence:

### 1. Create the Feature DbContext

```csharp
using Microsoft.EntityFrameworkCore;

namespace CopilotTest.YourFeature;

internal class YourFeatureDbContext : DbContext
{
    public YourFeatureDbContext(DbContextOptions<YourFeatureDbContext> options)
        : base(options)
    {
    }

    internal DbSet<YourEntity> YourEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // IMPORTANT: Set the schema name
        modelBuilder.HasDefaultSchema("YourFeature");

        modelBuilder.Entity<YourEntity>(entity =>
        {
            entity.ToTable("your_entities");
            entity.HasKey(e => e.Id);
            // Configure other properties...
        });
    }
}
```

### 2. Create a Design-Time Factory

Create a `YourFeatureDbContextFactory.cs` file:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.YourFeature;

internal class YourFeatureDbContextFactory : IDesignTimeDbContextFactory<YourFeatureDbContext>
{
    public YourFeatureDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<YourFeatureDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=copilottestdb;Username=copilottest;Password=copilottest");

        return new YourFeatureDbContext(optionsBuilder.Options);
    }
}
```

**Note**: This factory is only used at design time for migrations. The actual connection string is provided via configuration at runtime.

### 3. Register DbContext in the Installer

Update the feature's Installer class:

```csharp
public static class YourFeatureInstaller
{
    public static IServiceCollection AddYourFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<YourFeatureDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Register other services...

        return services;
    }
}
```

### 4. Create the Initial Migration

```bash
cd src/Features/YourFeature/CopilotTest.YourFeature/
dotnet ef migrations add InitialCreate \
    --context YourFeatureDbContext \
    --output-dir Migrations
```

### 5. Add InternalsVisibleTo (if needed for testing)

Create or update `AssemblyInfo.cs`:

```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CopilotTest.WebApi")]
[assembly: InternalsVisibleTo("CopilotTest.WebApi.HealthTests")]
```

## Schema Isolation Benefits

### 1. Independent Evolution
Each feature can evolve its data model independently without affecting other features.

### 2. Clear Boundaries
Database schema boundaries match bounded context boundaries in DDD.

### 3. Microservice Readiness
Easy to extract a feature into a separate microservice with its own database.

### 4. No Migration Conflicts
Teams can work on different features without migration conflicts.

### 5. Simplified Debugging
Schema names clearly indicate which feature owns which tables.

## Best Practices

### ✅ Do

- **Use descriptive migration names**: `AddOrderStatusColumn`, not `Migration1`
- **Review generated migrations**: Always check the generated SQL before applying
- **Commit migrations to version control**: Migrations are part of your code
- **Test migrations**: Apply migrations in a development environment first
- **Keep migrations small**: Create focused migrations for specific changes
- **Document complex migrations**: Add comments to explain non-obvious changes

### ❌ Don't

- **Never cross schema boundaries**: Don't create foreign keys between schemas
- **Don't modify applied migrations**: Create a new migration instead
- **Don't apply migrations manually in production**: Use automated deployment scripts
- **Don't share DbContext between features**: Each feature should have its own
- **Don't use the default schema**: Always specify a feature-specific schema

## Database Connection String

The connection string is configured in environment variables or `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=copilottestdb;Username=copilottest;Password=copilottest"
  }
}
```

**Note**: All features share the same database but use different schemas for isolation.

## Troubleshooting

### "Unable to create a 'DbContext' of type"

This usually means the design-time factory is missing or incorrectly configured. Ensure you have a `DbContextFactory` class implementing `IDesignTimeDbContextFactory<T>`.

### "Build failed"

Ensure your feature project builds successfully before creating migrations:

```bash
dotnet build
```

### "The name 'DbContext' does not exist in the current context"

Add the required package references to your feature's `.csproj`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.3">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />
```

## Verifying Schema Isolation

You can verify that schemas are correctly isolated by:

1. **Checking migration files**: Look for `schema: "FeatureName"` in the generated migrations
2. **Inspecting the database**: Connect to PostgreSQL and verify schemas exist:

```sql
SELECT schema_name
FROM information_schema.schemata
WHERE schema_name NOT IN ('pg_catalog', 'information_schema');

-- Should show: WebApi, Orders, etc.
```

3. **Viewing tables by schema**:

```sql
SELECT table_schema, table_name
FROM information_schema.tables
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
ORDER BY table_schema, table_name;
```

## Additional Resources

- [Entity Framework Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL Schema Documentation](https://www.postgresql.org/docs/current/ddl-schemas.html)
- [Project Architecture Guide](.github/copilot-instructions.md)

## Example: Complete Workflow

Here's a complete example of adding a new field to the Orders feature:

```bash
# 1. Navigate to the Orders feature
cd src/Features/Orders/CopilotTest.Orders/

# 2. (Make your code changes to Order.cs and OrdersDbContext.cs)

# 3. Create a migration
dotnet ef migrations add AddDeliveryDateToOrder \
    --context OrdersDbContext \
    --output-dir Migrations

# 4. Review the generated migration file in Migrations/
# (Check that it looks correct)

# 5. Apply the migration to update the database
dotnet ef database update --context OrdersDbContext

# 6. Test your changes

# 7. Commit the migration files to version control
git add Migrations/
git commit -m "Add delivery date field to Order"
```

## Summary

The feature-specific migration strategy ensures:
- **Clean separation** between bounded contexts
- **Independent evolution** of data models
- **Microservice readiness** for future scaling
- **Team autonomy** for parallel development
- **Clear ownership** through schema isolation

For more details on the overall architecture, see the [Architecture Guide](.github/copilot-instructions.md).
