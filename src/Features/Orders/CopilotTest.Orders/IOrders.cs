namespace CopilotTest.Orders;

public interface IOrders
{
    Task<OrderDto?> GetOrderAsync(int id);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<int> CreateOrderAsync(string customerName, decimal totalAmount);
}
