---
agent: agent
description: Scaffold a new feature following the CopilotTest DDD architecture
argument-hint: Provide the feature name (e.g., "Products", "Customers")
---

# Create a New Feature

Your goal is to scaffold a complete new feature library following the CopilotTest bounded-context architecture.

## Step 1: Gather Feature Information

If the feature name has **not** been provided by the user, ask:

> "What is the name of the new feature? Please provide a plural PascalCase noun (e.g., `Products`, `Customers`, `Invoices`)."

Do not proceed until you have a confirmed `[FeatureName]`.

Derive the following from the feature name:
- `[FeatureName]` – plural form (e.g., `Products`)
- `[FeatureNameSingular]` – singular form (e.g., `Product`)

## Step 2: Create the Feature Folder Structure

Create the following directory layout:

```
src/Features/[FeatureName]/
└── CopilotTest.[FeatureName]/
```

## Step 3: Create the C# Library Project File

Create `src/Features/[FeatureName]/CopilotTest.[FeatureName]/CopilotTest.[FeatureName].csproj` targeting **.NET 10**:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

Add any required NuGet references (e.g., `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`) as needed.

## Step 4: Create the Required Feature Components

Create all files inside `src/Features/[FeatureName]/CopilotTest.[FeatureName]/`.

### 4.1 Aggregate Root — `[FeatureNameSingular].cs`

```csharp
namespace CopilotTest.[FeatureName];

public class [FeatureNameSingular]
{
    public Guid Id { get; private set; }

    // Add domain properties and behaviour here
}
```

### 4.2 Collection Wrapper — `[FeatureName]Collection.cs`

Public APIs must return a collection wrapper, not a raw `IEnumerable<T>`:

```csharp
namespace CopilotTest.[FeatureName];

public class [FeatureName]Collection
{
    public IEnumerable<[FeatureNameSingular]> [FeatureName] { get; }

    public [FeatureName]Collection(IEnumerable<[FeatureNameSingular]> items)
    {
        [FeatureName] = items;
    }
}
```

### 4.3 Repository Interface — `I[FeatureName]Repository.cs`

```csharp
namespace CopilotTest.[FeatureName];

internal interface I[FeatureName]Repository
{
    Task<[FeatureName]Collection> GetAllAsync(CancellationToken cancellationToken = default);
    Task<[FeatureNameSingular]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync([FeatureNameSingular] entity, CancellationToken cancellationToken = default);
}
```

### 4.4 Repository Implementation — `[FeatureName]Repository.cs`

```csharp
namespace CopilotTest.[FeatureName];

internal class [FeatureName]Repository : I[FeatureName]Repository
{
    private readonly [FeatureName]DbContext _dbContext;

    public [FeatureName]Repository([FeatureName]DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<[FeatureName]Collection> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.[FeatureName].ToListAsync(cancellationToken);
        return new [FeatureName]Collection(items);
    }

    public async Task<[FeatureNameSingular]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.[FeatureName].FindAsync([id], cancellationToken);

    public async Task AddAsync([FeatureNameSingular] entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.[FeatureName].AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

### 4.5 Feature Service Interface — `I[FeatureName].cs`

```csharp
namespace CopilotTest.[FeatureName];

public interface I[FeatureName]
{
    Task<[FeatureName]Collection> GetAllAsync(CancellationToken cancellationToken = default);
    Task<[FeatureNameSingular]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### 4.6 Feature Service Implementation — `[FeatureName].cs`

```csharp
namespace CopilotTest.[FeatureName];

internal class [FeatureName] : I[FeatureName]
{
    private readonly I[FeatureName]Repository _repository;

    public [FeatureName](I[FeatureName]Repository repository)
    {
        _repository = repository;
    }

    public Task<[FeatureName]Collection> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    public Task<[FeatureNameSingular]?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);
}
```

### 4.7 Controller — `[FeatureName]Controller.cs`

```csharp
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest.[FeatureName];

[ApiController]
[Route("api/[controller]")]
public class [FeatureName]Controller : ControllerBase
{
    private readonly I[FeatureName] _service;

    public [FeatureName]Controller(I[FeatureName] service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }
}
```

### 4.8 DbContext — `[FeatureName]DbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;

namespace CopilotTest.[FeatureName];

internal class [FeatureName]DbContext : DbContext
{
    public [FeatureName]DbContext(DbContextOptions<[FeatureName]DbContext> options) : base(options)
    {
    }

    internal DbSet<[FeatureNameSingular]> [FeatureName] { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("[FeatureName]");

        modelBuilder.Entity<[FeatureNameSingular]>(entity =>
        {
            entity.ToTable("[FeatureName_lowercase]");
            entity.HasKey(e => e.Id);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(b =>
            b.MigrationsHistoryTable("__EFMigrationsHistory", "[FeatureName]"));
    }
}
```

### 4.9 Design-Time DbContext Factory — `[FeatureName]DbContextFactory.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CopilotTest.[FeatureName];

internal class [FeatureName]DbContextFactory : IDesignTimeDbContextFactory<[FeatureName]DbContext>
{
    public [FeatureName]DbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<[FeatureName]DbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=copilottestdb;Username=copilottest";

        optionsBuilder.UseNpgsql(connectionString, options =>
            options.MigrationsHistoryTable("__EFMigrationsHistory", "[FeatureName]"));

        return new [FeatureName]DbContext(optionsBuilder.Options);
    }
}
```

### 4.10 Installer — `[FeatureName]Installer.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CopilotTest.[FeatureName];

public static class [FeatureName]Installer
{
    public static IServiceCollection Add[FeatureName](
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<[FeatureName]DbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "[FeatureName]")));

        services.AddScoped<I[FeatureName], [FeatureName]>();
        services.AddScoped<I[FeatureName]Repository, [FeatureName]Repository>();

        return services;
    }
}
```

## Step 5: Add the Project to the Solution

Add the new project to `CopilotTest.slnx` by inserting a new `<Project>` entry:

```xml
<Project Path="src/Features/[FeatureName]/CopilotTest.[FeatureName]/CopilotTest.[FeatureName].csproj" />
```

## Step 6: Integrate with CopilotTest.WebApi

1. **Add project reference** in `src/CopilotTest.WebApi/CopilotTest.WebApi.csproj`:

   ```xml
   <ProjectReference Include="..\Features\[FeatureName]\CopilotTest.[FeatureName]\CopilotTest.[FeatureName].csproj" />
   ```

2. **Register in `Program.cs`**:

   ```csharp
   builder.Services.Add[FeatureName](builder.Configuration);
   ```

3. **Expose the controller assembly** (add to the existing `AddControllers()` chain or add a new call):

   ```csharp
   builder.Services.AddControllers()
       .AddApplicationPart(typeof([FeatureName]Controller).Assembly);
   ```

## Step 7: Generate the Initial EF Core Migration

After all files are created, instruct the user to run the following commands:

```bash
cd src/Features/[FeatureName]/CopilotTest.[FeatureName]/

dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Step 8: Verify the Expected File Structure

Confirm the following files exist:

```
src/Features/[FeatureName]/CopilotTest.[FeatureName]/
├── CopilotTest.[FeatureName].csproj
├── [FeatureNameSingular].cs              # Aggregate root
├── [FeatureName]Collection.cs            # Collection wrapper
├── I[FeatureName].cs                     # Service interface
├── [FeatureName].cs                      # Service implementation
├── I[FeatureName]Repository.cs           # Repository interface
├── [FeatureName]Repository.cs            # Repository implementation
├── [FeatureName]Controller.cs            # API controller
├── [FeatureName]DbContext.cs             # EF Core DbContext
├── [FeatureName]DbContextFactory.cs      # Design-time factory
└── [FeatureName]Installer.cs             # DI installer
```