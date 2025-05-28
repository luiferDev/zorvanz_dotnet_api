using Microsoft.AspNetCore.Mvc;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.Services;

namespace zorvanz_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController(AuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto request)
    {
        var success = await authService
            .RegisterUserAsync(request.Name, request.LastName, request.UserName, 
                request.Password, request.Email);
        if (!success)
            return BadRequest(new { Message = "El usuario ya existe." });

        return Ok(new { Message = "Usuario registrado exitosamente." });
    }
    
    [HttpPost("admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] UserRegisterDto request)
    {
        var success = await authService
            .RegisterAdminAsync(request.Name, request.LastName, request.UserName,
                request.Password, request.Email);
        if (!success)
            return BadRequest(new { Message = "El usuario ya existe." });
        var role = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).FirstOrDefault();


        return Ok(new { Message = $"Usuario registrado exitosamente. Role {role}" });
    }
}