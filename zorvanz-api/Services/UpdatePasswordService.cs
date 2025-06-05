using Microsoft.EntityFrameworkCore;
using zorvanz_api.Models.DTO.User;
using zorvanz_api.ZorvanzDbContext;

namespace zorvanz_api.Services;

public class UpdatePasswordService(ZorvanzContext context)
{
    public async Task<(bool success, string errorMessage)> UpdatePasswordAsync(Guid id, UpdatePasswordDto? updatePasswordDto)
    {
        try
        {
            var user = await context.Users.FirstAsync(u => u.Id == id);
        
            var oldPassword = updatePasswordDto.OldPassword = updatePasswordDto.OldPassword.Trim();
            var newPassword = updatePasswordDto.NewPassword = updatePasswordDto.NewPassword.Trim();
            var confirmPassword = updatePasswordDto.ConfirmPassword = updatePasswordDto.ConfirmPassword.Trim();

            var oldHashedPassword = PasswordHasher.HashPassword(oldPassword);
            if (string.IsNullOrWhiteSpace(oldPassword))
                return (false, "current password is required");
        
        
            if (string.IsNullOrWhiteSpace(newPassword))
                return (false, "New password is required");

        
            if (PasswordHasher.VerifyPassword(oldHashedPassword, user.Password))
            { 
                return (false, "Current password do not match");
            }

            if (newPassword != confirmPassword)
            {
                return (false, "New password do not match with confirm password input");
            } 
        
            var confirmPasswordHash = PasswordHasher.HashPassword(confirmPassword);
        
        
            var newPasswordHash = PasswordHasher.HashPassword(newPassword);
            user.Password = newPasswordHash;
            await context.SaveChangesAsync();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            // Aquí podrías agregar logging del error si lo necesitas
            return (false, "Update password error. Please, try again");
        }
    }
}