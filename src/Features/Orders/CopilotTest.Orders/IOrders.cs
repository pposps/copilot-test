namespace CopilotTest.Orders;

internal interface IOrders
{
    Task<Order?> GetOrderAsync(int id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order> CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(int id);
}
