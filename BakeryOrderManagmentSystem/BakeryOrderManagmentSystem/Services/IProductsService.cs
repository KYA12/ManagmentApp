public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductAsync(int id);
    Task<IEnumerable<ProductDto>> GetActiveProducts();
    Task<int> CreateProductAsync(ProductDto productDto);
    Task<int> UpdateProductAsync(int id, ProductDto productDto);
    Task<int> DeleteProductAsync(int id);
}