﻿namespace LW.Shared.DTOs.Grade;

public class GradeExamDto
{
    public int Id { get; set; }
    public int CompetitionId { get; set; }
    public string? CompetitionTitle { get; set; }
    public int Type { get; set; }
    public string? Title { get; set; }
    public string? Province { get; set; }
    public string? ExamEssayFile { get; set; }
    public string? ExamSolutionFile { get; set; }
    public string? UrlDownloadSolutionFile { get; set; }
    public string? Description { get; set; }
    public int Year { set; get; }
    public int NumberQuestion { set; get; }
    public bool IsActive { get; set; }
    public int GradeId { get; set; }
}