using Microsoft.AspNetCore.Mvc;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.Services;

namespace zorvanz_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(IAuthService authService): ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto login)
    {
        var token = await authService.LoginUserAsync(login.UserName, login.Password);
        if (token == null)
            return Unauthorized(new { Message = "Credenciales inválidas." });

        return Ok(new { Token = token });
    }
}