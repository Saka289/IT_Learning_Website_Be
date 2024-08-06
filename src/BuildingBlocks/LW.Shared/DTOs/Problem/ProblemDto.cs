﻿using LW.Shared.Enums;

namespace LW.Shared.DTOs.Problem;

public class ProblemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public string Difficulty { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public EStatusProblem Status { get; set; }
}