namespace CopilotTest.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrdersCollection
{
    public IEnumerable<OrderDto> Orders { get; set; } = Enumerable.Empty<OrderDto>();
}
