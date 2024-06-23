namespace LW.Shared.DTOs.Email;

public class VerifyEmailTokenDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; }
    public bool IsVerifyEmail = false;
}