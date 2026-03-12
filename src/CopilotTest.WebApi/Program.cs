using CopilotTest.Orders;
using CopilotTest.WebApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add DbContext
builder.Services.AddDbContext<WebApiHealthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add feature modules
builder.Services.AddOrders(builder.Configuration);

// Add controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(OrdersController).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.MapGet("/health", async (WebApiHealthDbContext dbContext) =>
{
    var healthStatus = await dbContext.Health.FirstOrDefaultAsync();
    return healthStatus != null ? Results.Ok(healthStatus.Status) : Results.Ok("unknown");
});

app.Run();

// Make Program class accessible for testing
public partial class Program { }

