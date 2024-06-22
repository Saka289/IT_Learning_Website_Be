namespace LW.Shared.DTOs.User;

public class LoginResponseUserDto
{
    public UserDto UserDto { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}