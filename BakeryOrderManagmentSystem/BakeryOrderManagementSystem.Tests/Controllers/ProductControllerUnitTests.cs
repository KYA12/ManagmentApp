using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace BakeryOrderManagementSystem.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(
                _productServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var products = new List<ProductDto>
        {
            new ProductDto { ProductId = 1, Name = "Product1", Price = 10.99m },
            new ProductDto { ProductId = 2, Name = "Product2", Price = 5.99m }
        };

            _productServiceMock.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturn500_WhenExceptionThrown()
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetAllProductsAsync())
                .ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var product = new ProductDto { ProductId = 1, Name = "Product1", Price = 10.99m };

            _productServiceMock.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Product1", returnedProduct.Name);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetProductAsync(It.IsAny<int>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task GetActiveProducts_ReturnsOkResult_WhenProductsExist()
        {
            // Arrange
            var activeProducts = new List<ProductDto>
            {
                new ProductDto { ProductId = 1, Name = "Product 1", Price = 10.0M },
                new ProductDto { ProductId = 2, Name = "Product 2", Price = 20.0M }
            };
            _productServiceMock.Setup(service => service.GetActiveProducts())
                .ReturnsAsync(activeProducts);

            // Act
            var result = await _controller.GetActiveProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(activeProducts);
        }

        [Fact]
        public async Task GetActiveProducts_Returns500_WhenExceptionIsThrown()
        {
            // Arrange
            _productServiceMock.Setup(service => service.GetActiveProducts())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetActiveProducts();

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult?.StatusCode.Should().Be(500);
            statusCodeResult?.Value.Should().Be("Internal server error");

            // Verify logging
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while fetching active products")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedAtAction_WhenProductCreated()
        {
            // Arrange
            var product = new ProductDto { ProductId = 1, Name = "Product1", Price = 10.99m };

            _productServiceMock.Setup(x => x.CreateProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
            Assert.Equal("Product1", returnedProduct.Name);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var product = new ProductDto { Price = 10.99m };

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent_WhenUpdateSuccessful()
        {
            // Arrange
            var product = new ProductDto { ProductId = 1, Name = "UpdatedProduct", Price = 12.99m };

            _productServiceMock.Setup(x => x.UpdateProductAsync(It.IsAny<int>(), It.IsAny<ProductDto>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenDeleteSuccessful()
        {
            // Arrange
            _productServiceMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _productServiceMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}