using AutoMapper;
using BakeryOrderManagmentSystem.API.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BakeryOrderManagementSystem.Tests.ProductServiceTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly Mock<IHubContext<NotificationHub>> _mockNotificationHub;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;

        public ProductServiceTests()
        {
            _mockMapper = new Mock<IMapper>();

            _mockNotificationHub = new Mock<IHubContext<NotificationHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            // Set up HubContext mock to return mocked clients
            _mockNotificationHub.Setup(hub => hub.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(clients => clients.All).Returns(_mockClientProxy.Object);


            _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns<Product>(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive,
                Description = p.Description
            });

            _mockMapper.Setup(m => m.Map<Product>(It.IsAny<ProductDto>())).Returns<ProductDto>(dto => new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Price = dto.Price,
                IsActive = dto.IsActive,
                Description = dto.Description
            });

            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                       .Returns<IEnumerable<Product>>(products => products.Select(p => new ProductDto
                       {
                           ProductId = p.ProductId,
                           Name = p.Name,
                           Price = p.Price,
                           IsActive = p.IsActive,
                           Description = p.Description
                       }));

            _mockMapper.Setup(m => m.Map<IEnumerable<Product>>(It.IsAny<IEnumerable<ProductDto>>()))
                       .Returns<IEnumerable<ProductDto>>(dtos => dtos.Select(dto => new Product
                       {
                           ProductId = dto.ProductId,
                           Name = dto.Name,
                           Price = dto.Price,
                           IsActive = dto.IsActive,
                           Description = dto.Description
                       }));

            _mockLogger = new Mock<ILogger<ProductService>>();
        }
        private BakeryDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BakeryDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new BakeryDbContext(options);
        }

        private async Task ClearDatabase(BakeryDbContext dbContext)
        {
            dbContext.Products.RemoveRange(dbContext.Products);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProducts_WhenProductsExist()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);

            var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Cake", Price = 10.0m, IsActive = true, Description = "Delicious cake" },
            new Product { ProductId = 2, Name = "Cookie", Price = 5.0m, IsActive = true, Description = "Crunchy cookie" }
        };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Cake");
            result.First().Description.Should().Be("Delicious cake");
        }

        [Fact]
        public async Task GetProductAsync_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);

            _context.Products.Add(new Product { ProductId = 1, Name = "Cake", Price = 10.0m, Description = "Delisious Cake", IsActive = true });

            await _context.SaveChangesAsync();

            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            var result = await _productService.GetProductAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(1);
            result.Name.Should().Be("Cake");
        }

        [Fact]
        public async Task CreateProductAsync_AddsProduct_WhenProductIsValid()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);
            var productDto = new ProductDto { Name = "New Cake", Price = 15.0m, Description = "Delicious new cake", IsActive = true };

            _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = "Delicious new cake",
                IsActive = true
            });

            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            var result = await _productService.CreateProductAsync(productDto);

            // Assert
            result.Should().BeGreaterThan(0);

            var product = await _context.Products.FindAsync(result);
            product.Should().NotBeNull();
            product?.Name.Should().Be("New Cake");
            product?.Price.Should().Be(15.0m);
        }

        [Fact]
        public async Task GetActiveProducts_ReturnsActiveProducts_WhenProductsExist()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);
            _context.Products.AddRange(
                new Product { ProductId = 1, Name = "Active Cake", Price = 10.0m, IsActive = true, Description = "Delicious Cake" },
                new Product { ProductId = 2, Name = "Inactive Cookie", Price = 5.0m, IsActive = false, Description = "Cookie" }
            );
            await _context.SaveChangesAsync();
            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            var result = await _productService.GetActiveProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Active Cake");
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProduct_WhenProductExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);
            _context.Products.Add(new Product { ProductId = 1, Name = "Cake", Price = 10.0m, Description = "Delicious Cake", IsActive = true, OrdersProducts = new List<OrdersProducts>() });
            await _context.SaveChangesAsync();

            var productDto = new ProductDto { ProductId = 1, Name = "Updated Cake", Price = 12.0m, Description = "Delicious Cake", IsActive = true };
            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            await _productService.UpdateProductAsync(1, productDto);

            // Assert
            var product = await _context.Products.FindAsync(1);
            product.Should().NotBeNull();
            product?.Name.Should().Be("Updated Cake");
            product?.Price.Should().Be(12.0m);
        }

        [Fact]
        public async Task DeleteProductAsync_SoftDeletesProduct_WhenProductExists()
        {
            // Arrange
            using var _context = CreateDbContext();
            await ClearDatabase(_context);
            _context.Products.Add(new Product { ProductId = 1, Name = "Cake", Price = 10.0m, Description = "Delicious Cake", IsActive = true });
            await _context.SaveChangesAsync();
            var _productService = new ProductService(_context, _mockMapper.Object, _mockLogger.Object, _mockNotificationHub.Object);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            result.Should().BeGreaterThan(0);

            var product = await _context.Products.FindAsync(1);
            product.Should().NotBeNull();
            product?.IsActive.Should().BeFalse();
        }
    }
}
