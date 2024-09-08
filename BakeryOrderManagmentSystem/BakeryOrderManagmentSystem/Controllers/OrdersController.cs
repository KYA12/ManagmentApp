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

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>A list of <see cref="OrderDto"/> objects.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="500">If there is an internal server error.</response>
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

    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>The <see cref="OrderDto"/> object with the specified ID.</returns>
    /// <response code="200">Returns the order with the specified ID.</response>
    /// <response code="404">If the order with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
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

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="orderDto">The order data to create.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="201">If the order is successfully created.</response>
    /// <response code="400">If the order data is invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
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

    /// <summary>
    /// Updates the status of an existing order.
    /// </summary>
    /// <param name="id">The ID of the order to update.</param>
    /// <param name="status">The new status of the order.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="204">If the order status is successfully updated.</response>
    /// <response code="400">If the status is null or empty.</response>
    /// <response code="404">If the order with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
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

    /// <summary>
    /// Deletes an existing order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="204">If the order is successfully deleted.</response>
    /// <response code="404">If the order with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        _logger.LogInformation($"Deleting order with id {id}");
        try
        {
            var result = await _orderService.DeleteOrderAsync(id);

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