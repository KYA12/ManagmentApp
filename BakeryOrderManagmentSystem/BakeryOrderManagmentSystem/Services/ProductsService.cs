using AutoMapper;
using BakeryOrderManagmentSystem.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly BakeryDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProductService(BakeryDbContext context, IMapper mapper, ILogger<ProductService> logger, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        _logger.LogInformation("Fetching all products");
        var products = await _context.Products.ToListAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        _logger.LogInformation($"Fetching product with id {id}");
        var product = await _context.Products.FindAsync(id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<int> CreateProductAsync(ProductDto productDto)
    {
        _logger.LogInformation($"Creating a new product with name {productDto.Name}");

        var product = _mapper.Map<Product>(productDto);
        _context.Products.Add(product);
        return await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<ProductDto>> GetActiveProducts()
    {
        _logger.LogInformation($"Fetching products with active status");
   
        var products = await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<int> UpdateProductAsync(int id, ProductDto productDto)
    {
        _logger.LogInformation($"Updating product with id {id}");
        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null)
        {
            _logger.LogWarning($"Product with id {id} not found");
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        var updatedProduct = _mapper.Map<Product>(productDto);

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.IsActive = updatedProduct.IsActive;
        existingProduct.ProductId = id;
     
        _context.Products.Update(existingProduct);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteProductAsync(int id)
    {
        _logger.LogInformation($"Deleting product with id {id}");
        var existingProduct = await _context.Products.FirstOrDefaultAsync(o => o.ProductId == id);

        if (existingProduct == null)
        {
            _logger.LogWarning($"Product with id {id} not found");
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }

        existingProduct.IsActive = false;
        _context.Products.Update(existingProduct);
        var result = await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveProductDeleted", id);

        return result;
    }
}