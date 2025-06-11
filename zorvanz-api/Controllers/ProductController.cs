using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zorvanz_api.Models.DTO;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(ILogger<ProductController> logger, IProductService productService)
    : ControllerBase
{
    /// <summary>
    /// Gets a paged list of products
    /// </summary>
    /// <param name="pageNumber">Page number (starts from 1)</param>
    /// <param name="pageSize">Number of items per page (default 9)</param>
    /// <returns>Paged list of products</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 9)
    {
        try
        {
            var products = await productService.GetProductsAsync(pageNumber, pageSize);
            return Ok(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    /// <summary>
    /// Gets a product by its ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        try
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound($"Product with id {id} not found");
            }

            if (product.Id == Guid.Empty)
            {
                return BadRequest("Invalid product ID");
            }

            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product with id {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }


    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="productDto">Product creation data</param>
    /// <returns>Created product details</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto productDto)
    {
        try
        {
            var product = await productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var deleted = await productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound($"Product with id {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product with id {Id}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }

    /// <summary>
    /// Partially updates a product
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="updates">Dictionary of field names and their new values</param>
    /// <returns>Updated product details</returns>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProductPartially(Guid id, [FromBody] UpdateProductDto updates)
    {
        try
        {
            var product = await productService.UpdateProductPartiallyAsync(id, updates);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product with id {Id}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }
}