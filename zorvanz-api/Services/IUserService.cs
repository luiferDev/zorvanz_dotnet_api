using zorvanz_api.Controllers;
using zorvanz_api.Models;
using zorvanz_api.Models.DTO.User;

namespace zorvanz_api.Services;

public interface IUserService
{
    Task<UserDto> GetUserAsync(string username);
    Task<UpdateUserDto> UpdateUserPartialAsync(string username, UpdateUserDto updateUser);
    Task<bool> DeleteUserAsync(Guid id);
}