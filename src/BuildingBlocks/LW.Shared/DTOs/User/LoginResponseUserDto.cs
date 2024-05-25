namespace LW.Shared.DTOs.User;

public class LoginResponseUserDto
{
    public UserDto UserDto { get; set; }
    public string token { get; set; }
}