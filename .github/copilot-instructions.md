# CopilotTest Project - GitHub Copilot Instructions

## Project Overview

This project is a GitHub Copilot test ride designed to explore and validate Copilot's capabilities in a .NET environment. The application is built as a modular web API using ASP.NET Core, with a focus on Domain-Driven Design (DDD) principles and clean architecture.

## Project Purpose

The primary goal is to test GitHub Copilot's effectiveness in:
- Understanding and following established architectural patterns
- Generating code that adheres to DDD and SOLID principles
- Creating well-structured, maintainable features in a microservices-ready architecture
- Maintaining consistency across bounded contexts

## Project Structure

### Main Entry Point
- **CopilotTest.WebApi**: The main web API project located at `src/CopilotTest.WebApi/`
- This is the single entry point for the application that hosts all feature endpoints
- Solution file: `CopilotTest.slnx` at the repository root

### Feature Organization

Features are organized as separate bounded contexts following DDD principles:

```
src/
├── CopilotTest.WebApi/          # Main API entry point
└── Features/                     # Root folder for all features
    ├── Orders/                   # Example feature folder
    │   └── CopilotTest.Orders/  # Feature library project
    └── [FeatureName]/
        └── CopilotTest.[FeatureName]/
```

### Rationale for Separate Feature Libraries

Each feature is implemented as a separate C# library project with its own controller. This architecture provides:
- **Independent Scaling**: Features can be scaled independently if needed
- **Microservice Foundation**: Feature libraries serve as the basis for future microservices if the need arises
- **Clear Boundaries**: Each feature is a bounded context with its own domain logic
- **Flexibility**: Easy to extract a feature into a separate service without major refactoring

## Feature Development Guidelines

### Creating a New Feature

When creating a new feature (e.g., "Orders"), follow these steps:

1. **Create Feature Folder Structure**
   ```
   src/Features/[FeatureName]/
   └── CopilotTest.[FeatureName]/
   ```

2. **Create C# Library Project**
   - Project name: `CopilotTest.[FeatureName]`
   - Location: `src/Features/[FeatureName]/CopilotTest.[FeatureName]/`
   - Target framework: .NET 10

3. **Add to Solution**
   - Add the new project to `CopilotTest.slnx`

### Required Feature Components

Each feature MUST contain the following components:

#### 1. Controller
- **Name**: `[FeatureName]Controller` (e.g., `OrdersController`)
- **Purpose**: Exposes HTTP endpoints for the feature
- **Injection**: Controller is registered in `CopilotTest.WebApi` Program.cs
- **Example**:
  ```csharp
  public class OrdersController : ControllerBase
  {
      // Controller implementation
  }
  ```

#### 2. Feature Service
- **Interface**: `I[FeatureName]` (e.g., `IOrders`)
- **Implementation**: `[FeatureName]` (e.g., `Orders`)
- **Purpose**: Contains business logic for the feature

#### 3. Aggregate Root
- **Name**: `[FeatureNameSingular]` (e.g., `Order` for Orders feature)
- **Purpose**: Represents the aggregate root in DDD terms
- **Responsibility**: Encapsulates domain logic and maintains consistency boundaries

#### 4. Repository
- **Interface**: `I[FeatureName]Repository` (e.g., `IOrdersRepository`)
- **Implementation**: `[FeatureName]Repository` (e.g., `OrdersRepository`)
- **Purpose**: Handles data persistence for the feature

#### 5. Installer
- **Name**: `[FeatureName]Installer` (e.g., `OrdersInstaller`)
- **Extension Method**: `Add[FeatureName](this IServiceCollection services, IConfiguration configuration)`
- **Purpose**: Registers all feature interfaces and implementations with dependency injection
- **Parameters**:
  - `IServiceCollection services` - for service registration
  - `IConfiguration configuration` - for accessing configuration (e.g., database connection strings)
- **Example**:
  ```csharp
  public static class OrdersInstaller
  {
      public static IServiceCollection AddOrders(
          this IServiceCollection services,
          IConfiguration configuration)
      {
          // Register services
          services.AddScoped<IOrders, Orders>();
          services.AddScoped<IOrdersRepository, OrdersRepository>();

          // Configure database if needed
          var connectionString = configuration.GetConnectionString("OrdersDb");
          // Additional configuration...

          return services;
      }
  }
  ```

## Development Principles

### SOLID Principles

Adhere strictly to SOLID principles:
- **Single Responsibility Principle (SRP)**: Each class should have one reason to change
- **Open/Closed Principle**: Open for extension, closed for modification
- **Liskov Substitution Principle**: Subtypes must be substitutable for their base types
- **Interface Segregation Principle**: No client should depend on methods it doesn't use
- **Dependency Inversion Principle**: Depend on abstractions, not concretions

### Clean Code Guidelines

- **Method Length**: Keep methods short and focused (ideally < 20 lines)
- **Class Length**: Keep classes small and cohesive
- **Method Decomposition**: Break long public methods into smaller private methods
- **Class Extraction**: If a class has too many responsibilities, extract logic into separate classes
- **Descriptive Names**: Use clear, descriptive names for classes, methods, and variables
  - Avoid abbreviations unless they're widely understood
  - Names should reveal intent
  - Use verbs for methods, nouns for classes

### Clean Architecture

Follow clean architecture principles:
- **Dependency Direction**: Dependencies point inward (toward domain)
- **Domain Independence**: Domain logic should not depend on infrastructure concerns
- **Separation of Concerns**: Clear separation between layers
- **Testability**: Design for testability with proper abstractions

### Domain-Driven Design (DDD)

Each feature is a **bounded context**:
- **Ubiquitous Language**: Use domain-specific terminology consistently
- **Aggregate Roots**: Maintain consistency boundaries with aggregate roots
- **Value Objects**: Use value objects for domain concepts without identity
- **Domain Events**: Consider domain events for cross-aggregate communication
- **Repository Pattern**: Abstract data access behind repository interfaces

## Code Style Conventions

- **Naming Conventions**: Follow C# naming conventions (PascalCase for public members, camelCase for private fields with underscore prefix)
- **Async/Await**: Use async/await for I/O-bound operations
- **Null Safety**: Leverage nullable reference types
- **Error Handling**: Use appropriate exception handling and validation
- **Logging**: Implement structured logging for observability
- **Collection Wrappers**: public API shouldn't return collections like IEnumerable<Order>, it should return collection wrapper like: OrdersCollection where OrdersCollection class have IEnumerable<Order> Orders property.
- **Encapsulation**: when possible use internal or private access modfier. 

## Integration with Main API

To integrate a new feature into `CopilotTest.WebApi`:

1. **Add Project Reference**
   ```xml
   <ProjectReference Include="..\Features\[FeatureName]\CopilotTest.[FeatureName]\CopilotTest.[FeatureName].csproj" />
   ```

2. **Register Feature in Program.cs**
   ```csharp
   builder.Services.Add[FeatureName](builder.Configuration);
   ```

3. **Add Controllers**
   ```csharp
   builder.Services.AddControllers()
       .AddApplicationPart(typeof([FeatureName]Controller).Assembly);
   ```

## Testing Guidelines

- Write unit tests for domain logic
- Write integration tests for controllers and repositories
- Write end-to-end (e2e) tests for complete feature workflows
- Follow AAA pattern (Arrange, Act, Assert)
- Use meaningful test names that describe the scenario
- Mock external dependencies appropriately in unit tests
- Use real dependencies in e2e tests to validate entire feature flows

## Example Feature Structure

For a feature called "Orders":

```
src/Features/Orders/CopilotTest.Orders/
├── OrdersController.cs
├── IOrders.cs
├── Orders.cs
├── Order.cs                          # aggregate root
├── IOrdersRepository.cs
├── OrdersRepository.cs
└── OrdersInstaller.cs
```

## Entity Framework Migrations Strategy

### Overview

Each feature in the architecture maintains its own Entity Framework Core migrations. This approach ensures:
- **Domain Boundaries**: Clear separation between bounded contexts
- **Independent Evolution**: Features can evolve their schemas independently
- **Microservice Readiness**: Easy extraction of features into separate services
- **Schema Isolation**: Each feature uses its own database schema/namespace

### Migration Requirements

#### 1. Feature DbContext

Each feature that requires data persistence MUST have its own DbContext:

**Naming Convention**: `[FeatureName]DbContext` (e.g., `OrdersDbContext`)

**Schema Configuration**: Use the `HasDefaultSchema` method to isolate tables:

```csharp
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

        // Configure migrations history table
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            // Additional configuration...
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Isolate migration history per feature
        optionsBuilder.UseNpgsql(b =>
            b.MigrationsHistoryTable("__EFMigrationsHistory_Orders", "Orders"));
    }
}
```

#### 2. Design-Time DbContext Factory

Each feature DbContext REQUIRES a design-time factory for EF Core tools to work:

**Naming Convention**: `[FeatureName]DbContextFactory`

**Implementation**:

```csharp
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
```

**Purpose**: Enables EF Core CLI tools to instantiate the DbContext without running the application

#### 3. Migration Generation

Generate migrations using the EF Core CLI tools:

```bash
# Navigate to the feature project directory
cd src/Features/Orders/CopilotTest.Orders/

# Generate a new migration
dotnet ef migrations add InitialCreate

# Apply migrations to the database
dotnet ef database update

# Remove the last migration (if not applied)
dotnet ef migrations remove
```

**Migration Naming**: Use descriptive names that indicate what changed (e.g., `AddOrderStatusColumn`, `CreateOrdersTable`)

#### 4. Migrations Location

Migrations MUST be stored within the feature project:

```
src/Features/Orders/CopilotTest.Orders/
├── Migrations/
│   ├── 20260312000000_InitialCreate.cs
│   └── 20260312000000_InitialCreate.Designer.cs
├── OrdersDbContext.cs
├── OrdersDbContextFactory.cs
└── ... (other feature files)
```

#### 5. Schema Naming Convention

Database schemas MUST match the feature name:

| Feature Name | Schema Name | Example Table |
|--------------|-------------|---------------|
| Orders       | Orders      | Orders.orders |
| Customers    | Customers   | Customers.customers |
| Products     | Products    | Products.products |

#### 6. Migration History Isolation

Each feature maintains its own migration history table within its schema:

- **Table Name**: `__EFMigrationsHistory`
- **Schema**: Feature schema (e.g., `Orders.__EFMigrationsHistory`)
- **Purpose**: Tracks which migrations have been applied for this feature

This isolation prevents migration conflicts between features and enables independent deployment.

#### 7. Feature Installer Registration

The feature installer MUST register the DbContext with dependency injection:

```csharp
public static class OrdersInstaller
{
    public static IServiceCollection AddOrders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "Orders")));

        // Register other services
        services.AddScoped<IOrders, Orders>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}
```

### Migration Workflow

#### Development Workflow

1. **Make Model Changes**: Update entity classes or add new entities
2. **Generate Migration**: Run `dotnet ef migrations add <MigrationName>` in the feature project
3. **Review Migration**: Check the generated migration code for correctness
4. **Apply Migration**: Run `dotnet ef database update` or let the application apply it on startup
5. **Test**: Verify the schema changes work as expected

#### Production Deployment

For production environments, consider:

- **Automated Migrations**: Apply migrations automatically on application startup
- **Manual Migrations**: Run migrations as part of deployment pipeline before application starts
- **Rollback Strategy**: Keep migration removal capability for rollback scenarios

### Best Practices

1. **Never Share DbContext**: Each feature has its own DbContext, never share between features
2. **Schema Isolation**: Always configure schema in `OnModelCreating` using `HasDefaultSchema`
3. **Migration History**: Always configure migration history table in `OnConfiguring` or when registering DbContext
4. **Connection Strings**: Use the same connection string for all features (database-level isolation via schemas)
5. **Factory Pattern**: Always implement `IDesignTimeDbContextFactory<T>` for EF tools support
6. **Descriptive Migrations**: Use clear, descriptive names for migrations
7. **Review Migrations**: Always review generated migration code before applying
8. **Test Migrations**: Test migrations in development environment before production

### Rationale

This migration strategy supports:

- **DDD Bounded Contexts**: Each feature's persistence is isolated
- **Microservice Extraction**: Easy to move a feature's DbContext and migrations to a separate service
- **Independent Deployment**: Features can be deployed independently without migration conflicts
- **Team Autonomy**: Different teams can work on different features without coordination
- **Clear Ownership**: Each feature owns its database schema and migrations

## Summary

This project demonstrates a modular, scalable architecture that:
- Follows DDD principles with bounded contexts
- Implements clean architecture and SOLID principles
- Provides a foundation for potential microservices migration
- Maintains clear separation of concerns
- Emphasizes testability and maintainability
- Uses descriptive naming and clean code practices

When working on this codebase, always consider:
- Is this method/class doing one thing well? (SRP)
- Can this be made more descriptive?
- Does this follow the established patterns?
- Is this testable?
- Does this maintain the bounded context boundaries?

## Language
No matter in which language you're receiving instructions - always use English language in all text.
