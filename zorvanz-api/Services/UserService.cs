using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using zorvanz_api.Models;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Services;

public class UserService(ZorvanzContext context) : IUserService
{
    public async Task<UserDto> GetUserAsync(string username)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);
        if (username == null || username != user.UserName)
        {
            throw new KeyNotFoundException($"User with username: {username} doesn't exist");
        }
        
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            Role = user.Role.ToString()
        };
        
    }

    public async Task<UpdateUserDto> UpdateUserPartialAsync(string username, UpdateUserDto updateUser)
    {
        ArgumentNullException.ThrowIfNull(updateUser);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            throw new KeyNotFoundException($"User with username: {username} doesn't exist");

        var anyUpdates = false;

        if (updateUser.Name != null)
        {
            user.Name = updateUser.Name;
            anyUpdates = true;
        }

        if (updateUser.LastName != null)
        {
            user.LastName = updateUser.LastName;
            anyUpdates = true;
        }

        if (updateUser.Email != null)
        {
            user.Email = updateUser.Email;
            anyUpdates = true;
        }

        if (updateUser.UserName != null)
        {
            if (updateUser.UserName.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long");

            user.UserName = updateUser.UserName;
            anyUpdates = true;
        }

        if (!anyUpdates)
            throw new ArgumentException("No valid updates provided");

        await context.SaveChangesAsync();

        return new UpdateUserDto()
        {
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName
        };
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
}