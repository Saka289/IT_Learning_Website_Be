using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Topic;

public class TopicUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required] 
    public string Description { get; set; }
    [Required] 
    public string Objectives { get; set; }
    [Required]
    public int DocumentId { get; set; }
    [Required] 
    public bool IsActive { get; set; }
    
    public int? ParentId { get; set; }

}