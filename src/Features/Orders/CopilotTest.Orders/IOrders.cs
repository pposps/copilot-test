namespace CopilotTest.Orders;

internal interface IOrders
{
    Task<OrdersCollection> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order> CreateOrderAsync(string customerName, decimal totalAmount);
}
