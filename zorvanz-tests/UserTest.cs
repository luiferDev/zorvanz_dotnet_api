using Microsoft.AspNetCore.Mvc;
using Moq;
using zorvanz_api.Controllers;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_tests;

public class UserTest
{
    
    
    [Fact]
    public async Task GetUsersAsync_ReturnsOkResult ()
    {
        // arrange
        var username = "testUser";
        var mockUserService = new Mock<IUserService>();
        var mockConext = new Mock<ZorvanzContext>();

        var expectedUsers = 
            new UserDto
            {
                Id = Guid.NewGuid(), Name = "Juan", LastName = "Lopez",
                Email = "luifer991@gmail.com", Role = "User", UserName = "testUser"
            };
        
        mockUserService
            .Setup(service => service.GetUserAsync(username))
            .ReturnsAsync(expectedUsers);

        var controller = new UserController(
            mockUserService.Object,
            null!,
            mockConext.Object
        );
        
        var result = await controller.GetUsersAsync(username);

        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsAssignableFrom<UserDto>(okResult.Value);
        Assert.Equal(expectedUsers, returnedUser);
    }
    
    [Fact]
    public async Task GetUsersAsync_ReturnsNotFound_WhenExceptionIsThrown()
    {
        // Arrange
        var username = "nonexistent";
        var mockUserService = new Mock<IUserService>();
        var mockContext = new Mock<ZorvanzContext>();

        mockUserService
            .Setup(service => service.GetUserAsync(username))
            .ThrowsAsync(new Exception("User not found"));

        var controller = new UserController(
            mockUserService.Object,
            null!,
            mockContext.Object
        );

        // Act
        var result = await controller.GetUsersAsync(username);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFoundResult.Value);
    }

}