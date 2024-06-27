namespace LW.Shared.DTOs.Index;

public class TopicIndexByLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public LessonIndexByLessonDto Lesson { get; set; }
}