public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> GetOrderAsync(int id);
    Task<int> CreateOrderAsync(OrderDto orderDto);
    Task<int> UpdateOrderStatusAsync(int id, string status);
    Task<int> DeleteOrderAsync(int id);
}