namespace LW.Shared.DTOs.Email;

public class VerifyEmailTokenDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public bool IsVerifyEmail = false;
}