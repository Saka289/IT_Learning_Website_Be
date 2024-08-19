namespace LW.Shared.DTOs.Index;

public class ChildTopicIndexByLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public LessonIndexByLessonDto Lesson { get; set; }
}