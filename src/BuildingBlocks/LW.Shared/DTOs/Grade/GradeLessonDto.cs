namespace LW.Shared.DTOs.Grade;

public class GradeLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public string FilePath { get; set; }
    public string PublicId { get; set; }
    public string UrlDownload { get; set; }
    public int TopicId { get; set; }
    public IEnumerable<GradeProblemDto> Problems { get; set; }
    public IEnumerable<GradeQuizDto> Quizzes { get; set; }
}