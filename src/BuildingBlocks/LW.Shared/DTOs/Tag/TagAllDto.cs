namespace LW.Shared.DTOs.Tag;

public class TagAllDto
{
    public IEnumerable<TagExamDto> Exams { get; set; }
    public IEnumerable<TagDocumentDto> Documents { get; set; }
    public IEnumerable<TagTopicDto> Topics { get; set; }
    public IEnumerable<TagLessonDto> lessons { get; set; }
    public IEnumerable<TagQuizDto> Quizzes { get; set; }
    public IEnumerable<TagProblemDto> Problems { get; set; }
}