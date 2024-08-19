using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Facebook;

public class FacebookSignInDto
{
    /// <summary>
    /// This token is generated from the client side. i.e. react, angular, flutter etc.
    /// </summary>
    [Required]
    public string AccessToken { get; set; }
}