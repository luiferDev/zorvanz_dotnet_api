using zorvanz_api.Models.DTO;

namespace zorvanz_api.Services;

public interface IProductService
{
    Task<PagedResponse<ProductDto>> GetProductsAsync(int pageNumber = 1, int pageSize = 9);
    Task<ProductDto> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task<bool> DeleteProductAsync(Guid id);
    Task<ProductDto> UpdateProductPartiallyAsync(Guid id, UpdateProductDto updates);
}