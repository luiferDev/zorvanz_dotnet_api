using Microsoft.EntityFrameworkCore;
using zorvanz_api.Models;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Services;

public class UserRepository(ZorvanzContext context)
{
    public async Task<User?> GetByUsernameAsync(string? username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}