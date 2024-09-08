using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace BakeryOrderManagementSystem.Tests.Controllers
{ 
    public class OrdersControllerTests
    {
        private readonly OrdersController _ordersController;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _ordersController = new OrdersController(_mockOrderService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOk_WithListOfOrders()
        {
            // Arrange
            var orders = new List<OrderDto> { new OrderDto { OrderId = 1, CustomerId = 2 } };
            _mockOrderService.Setup(service => service.GetAllOrdersAsync()).ReturnsAsync(orders);

            // Act
            var result = await _ordersController.GetAllOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsType<List<OrderDto>>(okResult.Value);
            Assert.Equal(orders.Count, returnedOrders.Count);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetAllOrdersAsync()).ThrowsAsync(new System.Exception());

            // Act
            var result = await _ordersController.GetAllOrders();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetOrder_ReturnsOk_WithOrder()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDto { OrderId = orderId, CustomerId = 2 };
            _mockOrderService.Setup(service => service.GetOrderAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(order.OrderId, returnedOrder.OrderId);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.GetOrderAsync(orderId)).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrder_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.GetOrderAsync(orderId)).ThrowsAsync(new System.Exception());

            // Act
            var result = await _ordersController.GetOrder(orderId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreated_WhenOrderIsCreated()
        {
            // Arrange
            var order = new OrderDto { OrderId = 1, CustomerId = 2 };
            _mockOrderService.Setup(service => service.CreateOrderAsync(order)).ReturnsAsync(1);

            // Act
            var result = await _ordersController.CreateOrder(order);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetOrder", createdAtActionResult.ActionName);
            Assert.Equal(order.OrderId, ((OrderDto)createdAtActionResult.Value).OrderId);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _ordersController.ModelState.AddModelError("CustomerId", "Required");

            // Act
            var result = await _ordersController.CreateOrder(new OrderDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var order = new OrderDto { OrderId = 1, CustomerId = 2 };
            _mockOrderService.Setup(service => service.CreateOrderAsync(order)).ThrowsAsync(new System.Exception());

            // Act
            var result = await _ordersController.CreateOrder(order);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var orderId = 1;
            var status = "Completed";
            _mockOrderService.Setup(service => service.UpdateOrderStatusAsync(orderId, status)).ReturnsAsync(1);

            // Act
            var result = await _ordersController.UpdateOrderStatus(orderId, status);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            var orderId = 1;
            var status = "Completed";
            _mockOrderService.Setup(service => service.UpdateOrderStatusAsync(orderId, status)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _ordersController.UpdateOrderStatus(orderId, status);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Act
            var result = await _ordersController.UpdateOrderStatus(1, string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Status cannot be null or empty", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.DeleteOrderAsync(orderId)).ReturnsAsync(1);

            // Act
            var result = await _ordersController.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.DeleteOrderAsync(orderId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _ordersController.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.DeleteOrderAsync(orderId)).ThrowsAsync(new System.Exception());

            // Act
            var result = await _ordersController.DeleteOrder(orderId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
