using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A list of <see cref="ProductDto"/> objects.</returns>
    /// <response code="200">Returns the list of all products.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        _logger.LogInformation("Getting all products");

        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all products");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>The <see cref="ProductDto"/> object with the specified ID.</returns>
    /// <response code="200">Returns the product with the specified ID.</response>
    /// <response code="404">If the product with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        _logger.LogInformation($"Getting product with id {id}");

        try
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found");
                return NotFound();
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching product with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves all active products.
    /// </summary>
    /// <returns>A list of <see cref="ProductDto"/> objects representing active products.</returns>
    /// <response code="200">Returns the list of active products.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
    {
        _logger.LogInformation("Fetching active products");

        try
        {
            var products = await _productService.GetActiveProducts();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching active products");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productDto">The product data to create.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="201">If the product is successfully created.</response>
    /// <response code="400">If the product data is invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid product data");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Creating product with name {productDto.Name}");

        try
        {
            var result = await _productService.CreateProductAsync(productDto);

            if (result > 0)
            {
                return CreatedAtAction(nameof(GetProduct), new { id = productDto.ProductId }, productDto);
            }
            else
            {
                _logger.LogWarning("Product creation failed");
                return StatusCode(500, "Product creation failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="productDto">The updated product data.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="204">If the product is successfully updated.</response>
    /// <response code="400">If the product data is invalid.</response>
    /// <response code="404">If the product with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid product data");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Updating product with id {id}");

        try
        {
            var result = await _productService.UpdateProductAsync(id, productDto);

            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Product updating failed");
                return StatusCode(500, "Product updating failed");
            }
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning($"Product with id {id} not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating product with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes an existing product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    /// <response code="204">If the product is successfully deleted.</response>
    /// <response code="404">If the product with the specified ID is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation($"Deleting product with id {id}");

        try
        {
            var result = await _productService.DeleteProductAsync(id);

            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Product deletion failed");
                return StatusCode(500, "Product deletion failed");
            }
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning($"Product with id {id} not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting product with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}