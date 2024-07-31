namespace LW.Shared.DTOs.ProgramLanguage;

public class ProgramLanguageUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BaseId { get; set; }
    public bool IsActive { get; set; }
}