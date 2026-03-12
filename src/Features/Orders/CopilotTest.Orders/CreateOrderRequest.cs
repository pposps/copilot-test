namespace CopilotTest.Orders;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
