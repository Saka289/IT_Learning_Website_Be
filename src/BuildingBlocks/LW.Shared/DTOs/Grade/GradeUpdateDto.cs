﻿namespace LW.Shared.DTOs.Grade;

public class GradeUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public int LevelId { get; set; }
}