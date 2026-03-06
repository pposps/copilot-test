using CopilotTest.WebApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

// Add DbContext
builder.Services.AddDbContext<WebApiHealthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", async (WebApiHealthDbContext dbContext) =>
{
    var healthStatus = await dbContext.Health.FirstOrDefaultAsync();
    return healthStatus != null ? Results.Ok(healthStatus.Status) : Results.Ok("unknown");
});

app.Run();

// Make Program class accessible for testing
public partial class Program { }

