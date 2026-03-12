using Microsoft.AspNetCore.Mvc;

namespace CopilotTest.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrders _orders;

    internal OrdersController(IOrders orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orders.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orders.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _orders.CreateOrderAsync(request.CustomerName, request.TotalAmount);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }
}

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
