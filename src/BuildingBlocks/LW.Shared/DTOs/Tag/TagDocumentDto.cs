namespace LW.Shared.DTOs.Tag;

public class TagDocumentDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public string BookCollection { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public int Edition { get; set; }
    public string TypeOfBook { get; set; }
    public bool IsActive { get; set; }
    public int GradeId { get; set; }
    public double? AverageRating { get; set; }
    public int TotalReviewer { get; set; }
}