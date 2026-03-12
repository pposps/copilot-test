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
    public async Task<ActionResult<OrdersCollection>> GetAll()
    {
        var orders = await _orders.GetAllOrdersAsync();
        var orderDtos = orders.Select(o => MapToDto(o));
        return Ok(new OrdersCollection { Orders = orderDtos });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orders.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(MapToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            TotalPrice = request.TotalPrice
        };

        var createdOrder = await _orders.CreateOrderAsync(order);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdOrder.Id },
            MapToDto(createdOrder));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _orders.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        await _orders.DeleteOrderAsync(id);
        return NoContent();
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            OrderDate = order.OrderDate,
            Status = order.Status
        };
    }
}
