using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using zorvanz_api.Controllers;
using zorvanz_api.Models.DTO;
using zorvanz_api.Services;
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace zorvanz_tests;

public class ProductControllerTest
{
    [Fact]
    public async Task GetProductAsync_ReturnsOkResult()
    {
        var mockProductService = new Mock<IProductService>();

        var product = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = "testProduct",
            Description = "testProduct",
            Category = new CategoryDto
            {
                Id = 1,
                CategoryName = "VELAS_AROMATICAS"
            },
            Price = 100000,
            ImageUrl = "testImageUrl",
            Popularity = 1,
            Stock = 100
        };

        var expectedProduct = new PagedResponse<ProductDto>(
            data: [product],
            pageNumber: 1,
            pageSize: 1,
            totalRecords: 1
            );

        mockProductService.Setup(service => service.GetProductsAsync(1, 1))
            .ReturnsAsync(expectedProduct);

        var productController = new ProductController(
            null!,
            mockProductService.Object
        );

        var result = await productController.GetProducts(1, 1);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsAssignableFrom<PagedResponse<ProductDto>>(okResult.Value);
        Assert.Equal(expectedProduct, returnedProduct);

        mockProductService.Verify(service => service.GetProductsAsync(1, 1));
    }
    
    [Fact]
    public async Task GetProductByIdAsync_ReturnsOkResult()
    {
        var mockProductService = new Mock<IProductService>();
        
        var product = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = "testProduct",
            Description = "testProduct",
            Category = new CategoryDto
            {
                Id = 1,
                CategoryName = "VELAS_AROMATICAS"
            },
            Price = 100000,
            ImageUrl = "testImageUrl",
            Popularity = 1,
            Stock = 100
        };
        
        mockProductService
            .Setup(service => service.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        var controller = new ProductController(
            null!,
            mockProductService.Object
            );
        
        var result = await controller.GetProductById(product.Id);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsAssignableFrom<ProductDto>(okResult.Value);
        Assert.Equal(product, returnedProduct);
        
        mockProductService.Verify(service => service.GetProductByIdAsync(product.Id));
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsNotFoundResult()
    {
        var mockProductService = new Mock<IProductService>();
        var mockLogger = new Mock<ILogger<ProductController>>();
        
        var id = Guid.Empty;

        
        mockProductService
            .Setup(service => service.GetProductByIdAsync(id))
            .ReturnsAsync(null as ProductDto);

        var controller = new ProductController(
            mockLogger.Object,
            mockProductService.Object
        );
        
        var result = await controller.GetProductById(id);
        Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.True(id == Guid.Empty);

        mockProductService
            .Verify(service => service.GetProductByIdAsync(id));
    }
    
    [Fact]
    public async Task GetProductByIdAsync_ReturnsBadRequest_WhenProductIdIsEmpty()
    {
        var mockService = new Mock<IProductService>();
        var mockLogger = new Mock<ILogger<ProductController>>();

        var product = new ProductDto { Id = Guid.Empty };

        mockService.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        var controller = new ProductController(mockLogger.Object, mockService.Object);

        var result = await controller.GetProductById(Guid.NewGuid());

        Assert.IsType<BadRequestObjectResult>(result.Result);
        
        mockService.Verify(s => s.GetProductByIdAsync(It.IsAny<Guid>()));

    }
    
    [Fact]
    public async Task GetProductByIdAsync_ReturnsInternalServerError_OnException()
    {
        var mockService = new Mock<IProductService>();
        var mockLogger = new Mock<ILogger<ProductController>>();
        var productId = Guid.NewGuid();

        mockService.Setup(s => s.GetProductByIdAsync(productId))
            .ThrowsAsync(new Exception("Unexpected error"));

        var controller = new ProductController(mockLogger.Object, mockService.Object);

        var result = await controller.GetProductById(productId);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the product", objectResult.Value);
        
        mockService.Verify(s => s.GetProductByIdAsync(productId));
    }
}