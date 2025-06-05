namespace zorvanz_api.Models.DTO.User;

public class UpdatePasswordDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}