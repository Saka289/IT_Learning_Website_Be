namespace LW.Shared.DTOs.Admin;

public class AdminDto
{
    public string ID { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    
    public IEnumerable<string> Roles { get; set; }

}