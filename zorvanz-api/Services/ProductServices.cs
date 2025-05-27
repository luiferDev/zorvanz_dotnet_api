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
            .AsNoTracking();

        var totalRecords = await query.CountAsync();
        
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(pr => new ProductDto
            {
                Id = pr.Id,
                Name = pr.Name,
                Description = pr.Description,
                Price = pr.Price,
                CategoryId = pr.Category!.Id,
                CategoryName = pr.Category.CategoryName,
                ImageUrl = pr.ImageUrl,
                Popularity = pr.Popularity,
                Stock = pr.Stock
            })
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
            CategoryId = product.Category?.Id,
            CategoryName = product.Category?.CategoryName,
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
            CategoryId = category.Id,
            CategoryName = category.CategoryName,
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

        if (updates.CategoryId.HasValue)
        {
            var category = await context.Categories.FindAsync(updates.CategoryId.Value);
            if (category == null)
                throw new KeyNotFoundException($"Category with id {updates.CategoryId} not found");
            product.CategoryId = updates.CategoryId.Value;
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
            CategoryId = product.Category?.Id,
            CategoryName = product.Category?.CategoryName,
            ImageUrl = product.ImageUrl,
            Popularity = product.Popularity,
            Stock = product.Stock
        };
    }
}