using Microsoft.EntityFrameworkCore;
using zorvanz_api.Models;
using zorvanz_api.Models.DTO;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Services;

public class ProductServices(ZorvanzContext context) : IProductService
{
    public async Task<PagedResponse<ProductDto>> GetProductsAsync(int pageNumber = 1, int pageSize = 9)
    {
        var query = context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.Popularity)
            .AsNoTracking();

        var totalRecords = await query.CountAsync();
        
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = new CategoryDto()
                    {
                        Id = p.Category!.Id,
                        CategoryName = p.Category.CategoryName
                    },
                    ImageUrl = p.ImageUrl,
                    Popularity = p.Popularity,
                    Stock = p.Stock
                }
            )
            .ToListAsync();

        return new PagedResponse<ProductDto>(products, pageNumber, pageSize, totalRecords);
    }

    public async Task<ProductDto> GetProductByIdAsync(Guid id)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            throw new KeyNotFoundException($"Product with id {id} not found");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = new CategoryDto()
            {
                Id = product.Category!.Id,
                CategoryName = product.Category.CategoryName
            },
            ImageUrl = product.ImageUrl,
            Popularity = product.Popularity,
            Stock = product.Stock
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var category = await context.Categories.FindAsync(productDto.CategoryId);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {productDto.CategoryId} not found");

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
            ImageUrl = productDto.ImageUrl,
            Popularity = productDto.popularity,
            Stock = productDto.Stock
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = new CategoryDto()
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            },
            ImageUrl = product.ImageUrl,
            Popularity = product.Popularity,
            Stock = product.Stock
        };
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return true;
    }
    

    public async Task<ProductDto> UpdateProductPartiallyAsync(Guid id, UpdateProductDto updates)
    {
        if (updates == null)
            throw new ArgumentNullException(nameof(updates));

        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            throw new KeyNotFoundException($"Product with id {id} not found");

        bool anyUpdates = false;

        if (updates.Name != null)
        {
            product.Name = updates.Name;
            anyUpdates = true;
        }

        if (updates.Description != null)
        {
            product.Description = updates.Description;
            anyUpdates = true;
        }

        if (updates.Price.HasValue)
        {
            if (updates.Price.Value <= 0)
                throw new ArgumentException("Price must be greater than zero");
            product.Price = updates.Price.Value;
            anyUpdates = true;
        }

        if (updates.CategoryId.HasValue && product.Category != null && product.Category.Id != updates.CategoryId.Value)
        {
            // Verificamos que la categoría existe
            var categoryExists = await context.Categories
                .AnyAsync(c => c.Id == updates.CategoryId.Value);
            
            if (!categoryExists)
                throw new KeyNotFoundException($"Category with id {updates.CategoryId} not found");
            
            // Solo actualizamos el CategoryId
            product.Category.Id = updates.CategoryId.Value;
            anyUpdates = true;
        }

        if (updates.ImageUrl != null)
        {
            product.ImageUrl = updates.ImageUrl;
            anyUpdates = true;
        }

        if (updates.Stock.HasValue)
        {
            if (updates.Stock.Value < 0)
                throw new ArgumentException("Stock cannot be negative");
            product.Stock = updates.Stock.Value;
            anyUpdates = true;
        }
        
        if (updates.Popularity > 0)
        {
            if (updates.Popularity < 0)
                throw new ArgumentException("Popularity cannot be negative");
            product.Popularity = updates.Popularity;
            anyUpdates = true;
        }

        if (!anyUpdates)
            throw new ArgumentException("No valid updates provided");

        await context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = new CategoryDto()
            {
                Id = product.Category!.Id,
                CategoryName = product.Category.CategoryName
            },
            ImageUrl = product.ImageUrl,
            Popularity = product.Popularity,
            Stock = product.Stock
        };
    }
}