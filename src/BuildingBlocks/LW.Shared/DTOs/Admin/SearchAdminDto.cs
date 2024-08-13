using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Admin;

public class SearchAdminDto : SearchRequestValue 
{
    public string? Role { get; set; }
}