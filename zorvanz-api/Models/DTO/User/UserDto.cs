namespace zorvanz_api.Models.DTO.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}