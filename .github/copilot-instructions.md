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

- **Public**: Controllers and Installer classes (controllers need to be discovered by the API, installers need to be accessible from the main API project)
- **Internal**: All other components (services, repositories, aggregate roots)
  - Note: Extension methods in installer classes are also `public`

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
