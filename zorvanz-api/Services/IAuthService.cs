using zorvanz_api.Models;

namespace zorvanz_api.Services;

public interface IAuthService
{
    Task<bool> RegisterUserAsync(string name, string lastname, string username, string password, string? email);
    Task<bool> RegisterAdminAsync(string name, string lastname, string username, string password, string? email);
    Task<string> LoginUserAsync(string username, string password);
    string GenerateJwtToken(User user);
}