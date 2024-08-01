using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Editorial;

public class EditorialCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile? Image { get; set; }
    public int ProblemId { get; set; }
}