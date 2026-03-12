namespace CopilotTest.Orders;

internal class OrdersCollection
{
    public IEnumerable<Order> Orders { get; set; } = Enumerable.Empty<Order>();
}
