using Microsoft.AspNetCore.Mvc;

namespace CopilotTest.Orders;

public class OrdersController : ControllerBase
{
    private readonly IOrders _orders;

    public OrdersController(IOrders orders)
    {
        _orders = orders;
    }

    [HttpGet("/orders/{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orders.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        var orders = await _orders.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPost("/orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderId = await _orders.CreateOrderAsync(request.CustomerName, request.TotalAmount);
        var order = await _orders.GetOrderAsync(orderId);

        if (order == null)
        {
            return StatusCode(500);
        }

        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, order);
    }
}
