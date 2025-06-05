using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    IUserService userService, 
    UpdatePasswordService updatePasswordService,
    ZorvanzContext context
    ) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] string username)
    {
        try
        {
            var users = await userService.GetUserAsync(username);
            return Ok(users);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    [HttpPatch]
    public async Task<IActionResult> UpdateUserAsync(
        [FromQuery] string username, 
        [FromBody] UpdateUserDto updateUser)
    {
        try
        {
            var user = await userService.UpdateUserPartialAsync(username, updateUser);
            return Ok(user);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        try
        {
            await userService.DeleteUserAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPatch]
    [Route("update-password")]
    public async Task<IActionResult> UpdatePasswordAsync(
        [FromQuery] Guid id, 
        [FromBody] UpdatePasswordDto? updatePassword)
    {
        try
        {
            var user = await context.Users.FindAsync(id);
            // Validar el ID
            if (id == Guid.Empty)
            {
                return BadRequest("El ID del usuario es inválido");
            }
            // Validar el DTO
            if (updatePassword == null)
            {
                return BadRequest("Los datos de actualización son requeridos");
            }

            if (updatePassword.NewPassword != updatePassword.ConfirmPassword)
            {
                return  BadRequest("Las contraseñas no coinciden");
            }
            if (PasswordHasher.VerifyPassword(updatePassword.OldPassword, user.Password) == false)
            {
                return BadRequest("La contraseña actual es incorrecta");
            }
            if (PasswordHasher.VerifyPassword(updatePassword.NewPassword, user.Password) == true)
            {
                return BadRequest("La nueva contraseña no puede ser igual a la anterior");
            }
            await updatePasswordService.UpdatePasswordAsync(id, updatePassword);
            return Ok("password Updated successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, "internal server error");
        }
    }
}

