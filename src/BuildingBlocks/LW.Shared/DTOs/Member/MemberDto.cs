﻿namespace LW.Shared.DTOs.Member;

public class MemberDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Dob { get; set; }
    public string? Image { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? LockOutEnd { get; set; }
    public IEnumerable<string> Roles { get; set; }
}