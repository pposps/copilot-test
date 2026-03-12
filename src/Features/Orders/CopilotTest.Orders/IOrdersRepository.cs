namespace CopilotTest.Orders;

internal interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task AddAsync(Order order);
}
