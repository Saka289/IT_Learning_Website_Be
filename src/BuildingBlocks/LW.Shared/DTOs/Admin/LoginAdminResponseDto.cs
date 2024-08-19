namespace LW.Shared.DTOs.Admin;

public class LoginAdminResponseDto
{
    public AdminDto Admin { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}