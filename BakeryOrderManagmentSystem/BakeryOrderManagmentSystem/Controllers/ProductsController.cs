using Microsoft.AspNetCore.Mvc;

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
            _logger.LogError(ex, $"Error occurred while deleting product with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}