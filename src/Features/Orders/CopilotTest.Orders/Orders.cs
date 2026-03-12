namespace CopilotTest.Orders;

internal class Orders : IOrders
{
    private readonly IOrdersRepository _repository;

    public Orders(IOrdersRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<int> CreateOrderAsync(string customerName, decimal totalAmount)
    {
        var order = new Order
        {
            CustomerName = customerName,
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        return await _repository.CreateAsync(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            Status = order.Status
        };
    }
}
