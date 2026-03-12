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

#### 1. Controller (Public)
- **Name**: `[FeatureName]Controller` (e.g., `OrdersController`)
- **Access Modifier**: `public`
- **Purpose**: Exposes HTTP endpoints for the feature
- **Injection**: Controller is registered in `CopilotTest.WebApi` Program.cs
- **Example**:
  ```csharp
  public class OrdersController : ControllerBase
  {
      // Controller implementation
  }
  ```

#### 2. Feature Service (Internal)
- **Interface**: `I[FeatureName]` (e.g., `IOrders`)
- **Implementation**: `[FeatureName]` (e.g., `Orders`)
- **Access Modifier**: `internal`
- **Purpose**: Contains business logic for the feature

#### 3. Aggregate Root (Internal)
- **Name**: `[FeatureNameSingular]` (e.g., `Order` for Orders feature)
- **Access Modifier**: `internal`
- **Purpose**: Represents the aggregate root in DDD terms
- **Responsibility**: Encapsulates domain logic and maintains consistency boundaries

#### 4. Repository (Internal)
- **Interface**: `I[FeatureName]Repository` (e.g., `IOrdersRepository`)
- **Implementation**: `[FeatureName]Repository` (e.g., `OrdersRepository`)
- **Access Modifier**: `internal`
- **Purpose**: Handles data persistence for the feature

#### 5. Installer (Public)
- **Name**: `[FeatureName]Installer` (e.g., `OrdersInstaller`)
- **Access Modifier**: `public` class with `public` extension method
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

### Access Modifier Rules

**Default Rule**: All internal components (services, repositories, aggregate roots, DbContext) should use `internal` access modifier to maintain proper encapsulation within the bounded context.

**Public Components**: Only the following MUST be `public`:
- Controllers (need to be discovered by ASP.NET Core)
- Installer classes and their extension methods (need to be accessible from the main API project)

#### Exception: Public Contract Types Rule

**CRITICAL EXCEPTION TO INTERNAL RULE**: Any type that is part of the **public API contract** MUST be `public`.

**Definition of Public Contract**: A type is part of the public API contract if it:
1. Is directly returned from a public method of a public class (actual return type, not wrapped in `IActionResult`)
2. Is a parameter of a public method of a public class (excluding infrastructure types injected via DI)
3. Is exposed through public properties or fields of a public class
4. Appears as a generic type argument in any of the above scenarios

**Examples of Public Contract Types**:

```csharp
// Example 1: DTO returned directly
public class OrdersController : ControllerBase
{
    // OrderDto is part of public contract - it MUST be public
    [HttpGet]
    public ActionResult<OrderDto> GetOrder(int id)
    {
        return new OrderDto { Id = id, Name = "Order 1" };
    }
}

public class OrderDto { } // MUST be public - it's in the contract
```

```csharp
// Example 2: Request model as parameter
public class OrdersController : ControllerBase
{
    // CreateOrderRequest is part of public contract - it MUST be public
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        // implementation
        return Ok();
    }
}

public class CreateOrderRequest { } // MUST be public - it's in the contract
```

```csharp
// Example 3: Generic type arguments
public class OrdersController : ControllerBase
{
    // PagedResult<OrderDto> means OrderDto is in public contract
    [HttpGet("list")]
    public ActionResult<PagedResult<OrderDto>> GetOrders()
    {
        return new PagedResult<OrderDto>();
    }
}

public class OrderDto { } // MUST be public
public class PagedResult<T> { } // MUST be public
```

**NOT Part of Public Contract**:

```csharp
// Infrastructure types injected via DI are NOT part of contract
public class OrdersController : ControllerBase
{
    // IOrdersService, ILogger can remain internal - they're infrastructure
    private readonly IOrdersService _ordersService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger)
    {
        _ordersService = ordersService; // Not exposed externally
        _logger = logger; // Not exposed externally
    }
}

internal interface IOrdersService { } // Can be internal - not in public contract
```

```csharp
// Types only used internally (not returned or accepted) are NOT part of contract
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetOrder(int id)
    {
        // Even though we create InternalOrderEntity, it's not in the signature
        var order = new InternalOrderEntity();

        // We convert to DTO before returning
        return Ok(new { Id = order.Id, Name = order.Name });
    }
}

internal class InternalOrderEntity { } // Can be internal - not directly exposed
```

**Special Case - Minimal APIs**:

In ASP.NET Minimal APIs (like `app.MapGet`), similar rules apply:
- Types in the delegate signature that are part of the API contract (returned or accepted from HTTP) MUST be public
- Infrastructure types injected from DI (like `DbContext`, services) can remain internal

```csharp
// Minimal API example
app.MapGet("/orders/{id}",
    async (int id, IOrdersService ordersService) => // IOrdersService can be internal
{
    var order = await ordersService.GetOrderAsync(id);
    return Results.Ok(new OrderDto { Id = order.Id }); // OrderDto MUST be public if returned
});

internal interface IOrdersService { } // OK - injected via DI
public class OrderDto { } // MUST be public - returned to HTTP clients
```

**Summary for AI Agents**:
1. **Start with internal by default** for all domain and infrastructure types
2. **Make it public** if and only if:
   - It's a Controller or Installer class, OR
   - It's directly exposed in the public API contract (returned to or accepted from HTTP clients)
3. **Keep it internal** if it's only used for:
   - Dependency injection (services, repositories, DbContext)
   - Internal implementation details
   - Data transformation (entity to DTO conversion)

This ensures proper encapsulation while maintaining a valid public API surface.

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
├── OrdersController.cs              # public
├── IOrders.cs                        # internal
├── Orders.cs                         # internal
├── Order.cs                          # internal (aggregate root)
├── IOrdersRepository.cs             # internal
├── OrdersRepository.cs              # internal
└── OrdersInstaller.cs               # public
```

## Entity Framework Migrations Strategy

### Overview

Each feature maintains its own Entity Framework (EF) migrations to ensure independent evolution, clear domain boundaries, and facilitate potential microservice extraction. This approach aligns with Domain-Driven Design (DDD) principles by maintaining strict bounded context boundaries.

### Feature-Specific Database Contexts

Each feature that requires data persistence MUST have:

1. **Feature-Specific DbContext** (Internal)
   - **Name**: `[FeatureName]DbContext` (e.g., `OrdersDbContext`)
   - **Access Modifier**: `internal`
   - **Purpose**: Manages database operations for the feature's bounded context
   - **Schema Configuration**: Must configure tables to use feature-specific schema

2. **Schema Naming Convention**
   - Each feature's tables MUST be placed in a schema named after the feature
   - Schema name: `[FeatureName]` (e.g., `Orders`, `Inventory`, `Payments`)
   - Exception: The main WebApi's health check uses `WebApi` schema

### DbContext Requirements

Each feature's DbContext MUST:

#### 1. Inherit from DbContext
```csharp
internal class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    internal DbSet<Order> Orders { get; set; }
}
```

#### 2. Configure Schema in OnModelCreating
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Set default schema for all entities in this context
    modelBuilder.HasDefaultSchema("Orders");

    // Configure entities
    modelBuilder.Entity<Order>(entity =>
    {
        entity.ToTable("orders"); // Table name within the Orders schema
        entity.HasKey(e => e.Id);
        // Additional configuration...
    });
}
```

### Migration Management

#### Creating Migrations

Each feature's migrations are created and managed independently:

```bash
# Navigate to the feature project directory
cd src/Features/Orders/CopilotTest.Orders/

# Create a new migration
dotnet ef migrations add InitialCreate \
    --context OrdersDbContext \
    --output-dir Migrations

# Update the database
dotnet ef database update \
    --context OrdersDbContext \
    --connection "YourConnectionString"
```

#### Migration Location
- Migrations MUST be stored in a `Migrations` folder within the feature project
- Path: `src/Features/[FeatureName]/CopilotTest.[FeatureName]/Migrations/`

#### Migration History Isolation
- Each feature maintains its own migration history in the `__EFMigrationsHistory` table
- The schema column in the history table differentiates between features
- This ensures features can evolve independently

### Feature Installer Updates

The feature Installer class MUST apply migrations during startup (development/staging only):

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
                configuration.GetConnectionString("DefaultConnection")));

        // Register other services
        services.AddScoped<IOrders, Orders>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}
```

**Note**: Automatic migration application should be controlled via configuration and typically disabled in production environments.

### Required Feature Component Update

Add to the list of required feature components:

#### 6. DbContext (Internal) - If Feature Requires Persistence
- **Name**: `[FeatureName]DbContext` (e.g., `OrdersDbContext`)
- **Access Modifier**: `internal`
- **Purpose**: Manages database operations for the feature
- **Schema**: Must use `[FeatureName]` as the default schema
- **Location**: Same project as other feature components

### Benefits of This Approach

1. **Independent Evolution**: Features can evolve their data models independently
2. **Clear Boundaries**: Database schema boundaries match bounded context boundaries
3. **Microservice Readiness**: Easy to extract a feature with its own database
4. **Conflict Avoidance**: No migration conflicts between features
5. **Team Autonomy**: Different teams can work on different features without coordination
6. **Simplified Debugging**: Schema names make it clear which feature owns which tables

### Best Practices

1. **Never Cross Schema Boundaries**: Features should never reference tables from other schemas
2. **Use Repository Pattern**: All database access should go through the feature's repository
3. **Migration Naming**: Use descriptive migration names (e.g., `AddOrderStatusColumn`)
4. **Review Migrations**: Always review generated migrations before applying them
5. **Version Control**: Always commit migration files to version control
6. **Testing**: Test migrations in a development environment before production
7. **Rollback Plan**: Understand how to rollback migrations if needed

### Example Feature Structure with DbContext

For a feature called "Orders":

```
src/Features/Orders/CopilotTest.Orders/
├── OrdersController.cs              # public
├── IOrders.cs                        # internal
├── Orders.cs                         # internal
├── Order.cs                          # internal (aggregate root)
├── OrdersDbContext.cs               # internal (NEW)
├── IOrdersRepository.cs             # internal
├── OrdersRepository.cs              # internal
├── OrdersInstaller.cs               # public
└── Migrations/                       # NEW
    ├── 20260312_InitialCreate.cs
    └── 20260312_InitialCreate.Designer.cs
```

### Working with Multiple Features

When multiple features have their own DbContexts:

1. Each feature registers its own DbContext in its Installer
2. Each DbContext uses its own schema
3. Migrations are applied independently
4. The same connection string can be used (different schemas, same database)
5. Features remain isolated at the database level

### Migration Commands Reference

```bash
# List all migrations for a context
dotnet ef migrations list --context OrdersDbContext

# Remove the last migration (if not applied)
dotnet ef migrations remove --context OrdersDbContext

# Generate SQL script for a migration
dotnet ef migrations script --context OrdersDbContext

# Update to a specific migration
dotnet ef database update MigrationName --context OrdersDbContext

# Rollback all migrations
dotnet ef database update 0 --context OrdersDbContext
```

## Summary

This project demonstrates a modular, scalable architecture that:
- Follows DDD principles with bounded contexts
- Implements clean architecture and SOLID principles
- Provides a foundation for potential microservices migration
- Maintains clear separation of concerns
- Emphasizes testability and maintainability
- Uses descriptive naming and clean code practices
- Implements feature-specific EF migrations with schema isolation

When working on this codebase, always consider:
- Is this method/class doing one thing well? (SRP)
- Can this be made more descriptive?
- Does this follow the established patterns?
- Is this testable?
- Does this maintain the bounded context boundaries?
- Are database migrations isolated to the feature's schema?

## Language
No matter in which language you're receiving instructions - always use English language in all text.
