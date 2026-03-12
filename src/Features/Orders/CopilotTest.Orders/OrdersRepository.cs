using Microsoft.EntityFrameworkCore;

namespace CopilotTest.Orders;

internal class OrdersRepository : IOrdersRepository
{
    private readonly OrdersDbContext _context;

    public OrdersRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}
