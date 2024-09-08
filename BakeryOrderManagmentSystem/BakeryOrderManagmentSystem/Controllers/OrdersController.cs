using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        _logger.LogInformation("Getting all orders");
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all orders");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        _logger.LogInformation($"Getting order with id {id}");
        try
        {
            var order = await _orderService.GetOrderAsync(id);

            if (order == null)
            {
                _logger.LogWarning($"Order with id {id} not found");
                return NotFound();
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching order with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] OrderDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid order data");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Creating order for customer {orderDto.CustomerId}");
        try
        {
            var result = await _orderService.CreateOrderAsync(orderDto);
            
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetOrder), new { id = orderDto.OrderId }, orderDto);
            }
            else
            {
                _logger.LogWarning("Order creation failed");
                return StatusCode(500, "Order creation failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateOrderStatus(int id, [FromQuery] string status)
    {
        if (string.IsNullOrEmpty(status))
        {
            _logger.LogWarning("Status cannot be null or empty");
            return BadRequest("Status cannot be null or empty");
        }

        _logger.LogInformation($"Updating status for order with id {id}");
        try
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);

            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Order updating failed");
                return StatusCode(500, "Order updating failed");
            }
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning($"Order with id {id} not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating status for order with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        _logger.LogInformation($"Deleting order with id {id}");
        try
        {
            var result =  await _orderService.DeleteOrderAsync(id);

            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Order deleting failed");
                return StatusCode(500, "Order deleting failed");
            }
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning($"Order with id {id} not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting order with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}