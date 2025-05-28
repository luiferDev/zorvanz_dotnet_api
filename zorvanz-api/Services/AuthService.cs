using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using zorvanz_api.Models;

namespace zorvanz_api.Services;

public class AuthService(UserRepository userRepository, IConfiguration configuration)
{
    public async Task<bool> RegisterUserAsync(string name, string lastname, string username, string password, string? email)
    {
        if (await userRepository.GetByUsernameAsync(username) != null)
            return false; // Usuario ya existe.

        var hashedPassword = PasswordHasher.HashPassword(password);

        var user = new User
        {
            Name = name,
            LastName = lastname,
            UserName = username,
            Password = hashedPassword,
            Role = Role.User,
            Email = email
        };

        await userRepository.AddUserAsync(user);
        return true;
    }
    
    public async Task<bool> RegisterAdminAsync(string name, string lastname, string username, string password, string? email)
    {
        if (await userRepository.GetByUsernameAsync(username) != null)
            return false; // Usuario ya existe.

        var hashedPassword = PasswordHasher.HashPassword(password);

        var user = new User
        {
            Name = name,
            LastName = lastname,
            UserName = username,
            Password = hashedPassword,
            Role = Role.Admin,
            Email = email
        };

        await userRepository.AddUserAsync(user);
        return true;
    }

    public async Task<string> LoginUserAsync(string username, string password)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        
        if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
            return "Credenciales inválidas";

        // Aquí puedes generar un token
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("Jwt");

        var tokenHandler = new JwtSecurityTokenHandler();
        var byteKey = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException());
        var tokenDes = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidOperationException()), 
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]), 
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDes);
        return tokenHandler.WriteToken(token);
    }
}