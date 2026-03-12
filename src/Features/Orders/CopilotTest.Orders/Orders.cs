namespace CopilotTest.Orders;

internal class Orders : IOrders
{
    private readonly IOrdersRepository _repository;

    public Orders(IOrdersRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrdersCollection> GetAllOrdersAsync()
    {
        var orders = await _repository.GetAllAsync();
        return new OrdersCollection { Orders = orders };
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(string customerName, decimal totalAmount)
    {
        var order = new Order
        {
            CustomerName = customerName,
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        await _repository.AddAsync(order);
        return order;
    }
}
