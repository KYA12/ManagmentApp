using AutoMapper;
using BakeryOrderManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
public class OrderService : IOrderService
{
    private readonly BakeryDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(BakeryDbContext context, IMapper mapper, ILogger<OrderService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        _logger.LogInformation("Fetching all orders");
        var orders = await _context.Orders
                                   .Include(o => o.OrdersProducts)
                                   .ThenInclude(op => op.Product)
                                   .ToListAsync();

        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return orderDtos ?? Enumerable.Empty<OrderDto>();
    }

    public async Task<OrderDto> GetOrderAsync(int id)
    {
        _logger.LogInformation($"Fetching order with id {id}");

        // Fetch the order including order products and related products
        var order = await _context.Orders
            .Include(o => o.OrdersProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            _logger.LogWarning($"Order with id {id} not found");
            throw new KeyNotFoundException($"Order with id {id} not found.");
        }

        var orderDto = _mapper.Map<OrderDto>(order);

        foreach (var orderProductDto in orderDto.OrdersProducts)
        {
            var product = order.OrdersProducts
                .FirstOrDefault(op => op.OrderProductId == orderProductDto.OrderProductId)?
                .Product;

            if (product != null)
            {
                orderProductDto.ProductName = product.Name;
            }
        }

        return orderDto;
    }

    public async Task<int> CreateOrderAsync(OrderDto orderDto)
    {
        _logger.LogInformation($"Creating a new order for customer {orderDto.CustomerId}");

        var order = _mapper.Map<Order>(orderDto);

        if (orderDto.OrdersProducts != null && orderDto.OrdersProducts.Any())
        {
            var orderProducts = _mapper.Map<List<OrdersProducts>>(orderDto.OrdersProducts);
            order.OrdersProducts = orderProducts;
        }

        _context.Orders.Add(order);
        return await _context.SaveChangesAsync();
    }
    public async Task<int> UpdateOrderStatusAsync(int id, string status)
    {
        _logger.LogInformation($"Updating order with id {id}");
        var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        if (existingOrder == null)
        {
            _logger.LogWarning($"Order with id {id} not found");
            throw new KeyNotFoundException($"Order with id {id} not found.");
        }

        if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
        {
            existingOrder.Status = parsedStatus;
            _context.Orders.Update(existingOrder);
            return await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning($"Invalid status value: {status}");
            throw new ArgumentException($"Invalid status value: {status}");
        }
    }

    public async Task<int> DeleteOrderAsync(int id)
    {
        _logger.LogInformation($"Deleting order with id {id}");
        var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

        if (existingOrder == null)
        {
            _logger.LogWarning($"Order with id {id} not found");
            throw new KeyNotFoundException($"Order with id {id} not found.");
        }

        _context.Orders.Remove(existingOrder);
        return await _context.SaveChangesAsync();
    }
}