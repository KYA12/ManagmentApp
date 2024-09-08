using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BakeryOrderManagmentSystem.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace BakeryOrderManagementSystem.Tests.OrderServiceTests
{


    public class OrderServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<OrderService>> _mockLogger;

        public OrderServiceTests()
        {
            // Setup Mocks for AutoMapper and Logger
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
                       .Returns<Order>(o => new OrderDto
                       {
                           OrderId = o.OrderId,
                           CustomerId = o.CustomerId,
                           Status = o.Status.ToString(),
                           OrderDate = o.OrderDate,
                           OrdersProducts = o.OrdersProducts?.Select(op => new OrderProductDto
                           {
                               OrderProductId = op.OrderProductId,
                               OrderId = op.OrderId,
                               ProductId = op.ProductId,
                               Quantity = op.Quantity
                           }).ToList()
                       });

            _mockMapper.Setup(m => m.Map<Order>(It.IsAny<OrderDto>()))
                       .Returns<OrderDto>(dto => new Order
                       {
                           OrderId = dto.OrderId,
                           CustomerId = dto.CustomerId,
                           Status = Enum.Parse<OrderStatus>(dto.Status),
                           OrderDate = dto.OrderDate
                       });

            _mockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
                       .Returns<IEnumerable<Order>>(orders => orders.Select(o => new OrderDto
                       {
                           OrderId = o.OrderId,
                           CustomerId = o.CustomerId,
                           Status = o.Status.ToString(),
                           OrderDate = o.OrderDate,
                           OrdersProducts = o.OrdersProducts?.Select(op => new OrderProductDto
                           {
                               OrderProductId = op.OrderProductId,
                               OrderId = op.OrderId,
                               ProductId = op.ProductId,
                               Quantity = op.Quantity
                           }).ToList()
                       }));

            _mockMapper.Setup(m => m.Map<IEnumerable<Order>>(It.IsAny<IEnumerable<OrderDto>>()))
                       .Returns<IEnumerable<OrderDto>>(dtos => dtos.Select(dto => new Order
                       {
                           OrderId = dto.OrderId,
                           CustomerId = dto.CustomerId,
                           Status = Enum.Parse<OrderStatus>(dto.Status),
                           OrderDate = dto.OrderDate
                       }));

            _mockLogger = new Mock<ILogger<OrderService>>();
        }

        private BakeryDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BakeryDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new BakeryDbContext(options);
        }

        private void ClearDatabase(BakeryDbContext dbContext)
        {
            dbContext.Orders.RemoveRange(dbContext.Orders);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsOrders_WhenOrdersExist()
        {
            // Arrange
            using var _context = CreateDbContext();
            ClearDatabase(_context);

            var dateTime = DateTime.Now;

            _context.Orders.AddRange(new List<Order>
            {
                new Order { OrderId = 1, CustomerId = 1, Status = OrderStatus.Pending, OrderDate = dateTime },
                new Order { OrderId = 2, CustomerId = 2, Status = OrderStatus.Completed, OrderDate = dateTime }
            });

            await _context.SaveChangesAsync();

            var _orderService = new OrderService(_context, _mockMapper.Object, _mockLogger.Object);

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().OrderId.Should().Be(1);
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsOrder_WhenOrderExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            ClearDatabase(_context);

            var dateTime = DateTime.Now;
            _context.Orders.Add(new Order { OrderId = 1, CustomerId = 1, Status = OrderStatus.Pending, OrderDate = dateTime });

            await _context.SaveChangesAsync();

            var _orderService = new OrderService(_context, _mockMapper.Object, _mockLogger.Object);

            // Act
            var result = await _orderService.GetOrderAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(1);
            result.Status.Should().Be("Pending");
        }

        [Fact]
        public async Task CreateOrderAsync_AddsOrder_WhenOrderIsValid()
        {
            // Arrange
            using var _context = CreateDbContext();
            ClearDatabase(_context);

            var _orderService = new OrderService(_context, _mockMapper.Object, _mockLogger.Object);
            var orderDto = new OrderDto { CustomerId = 3, Status = "Pending", OrderDate = DateTime.Now };

            _mockMapper.Setup(m => m.Map<Order>(orderDto)).Returns(new Order
            {
                CustomerId = orderDto.CustomerId,
                Status = Enum.Parse<OrderStatus>(orderDto.Status),
                OrderDate = orderDto.OrderDate,
                OrderId = orderDto.OrderId
            });

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            result.Should().BeGreaterThan(0);
            var createdOrder = await _context.Orders.FindAsync(result);
            createdOrder.Should().NotBeNull();
            createdOrder?.CustomerId.Should().Be(3);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesOrderStatus_WhenOrderExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            ClearDatabase(_context);

            var dateTime = DateTime.Now;

            _context.Orders.Add(new Order { OrderId = 1, CustomerId = 1, Status = OrderStatus.Pending, OrderDate = dateTime });
            await _context.SaveChangesAsync();

            var _orderService = new OrderService(_context, _mockMapper.Object, _mockLogger.Object);

            // Act
            await _orderService.UpdateOrderStatusAsync(1, "Completed");

            // Assert
            var updatedOrder = await _context.Orders.FindAsync(1);
            updatedOrder?.Status.Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public async Task DeleteOrderAsync_RemovesOrder_WhenOrderExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            ClearDatabase(_context);

            var dateTime = DateTime.Now;

            _context.Orders.Add(new Order { OrderId = 1, CustomerId = 1, Status = OrderStatus.Pending, OrderDate = dateTime });
            await _context.SaveChangesAsync();

            var _orderService = new OrderService(_context, _mockMapper.Object, _mockLogger.Object);

            // Act
            var result = await _orderService.DeleteOrderAsync(1);

            // Assert
            result.Should().BeGreaterThan(0);
            var deletedOrder = await _context.Orders.FindAsync(1);
            deletedOrder.Should().BeNull();
        }
    }
}