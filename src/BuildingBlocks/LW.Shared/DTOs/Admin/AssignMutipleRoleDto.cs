namespace LW.Shared.DTOs.Admin;

public class AssignMutipleRoleDto

{
    public string UserId { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}