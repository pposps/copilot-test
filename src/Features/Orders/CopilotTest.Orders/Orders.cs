namespace CopilotTest.Orders;

internal class Orders : IOrders
{
    private readonly IOrdersRepository _repository;

    public Orders(IOrdersRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.OrderDate = DateTime.UtcNow;
        order.Status = "Pending";
        return await _repository.AddAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await _repository.UpdateAsync(order);
    }

    public async Task DeleteOrderAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
