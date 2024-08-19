namespace LW.Shared.DTOs.Admin;

public class AssignMultipleRoleDto

{
    public string UserId { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}