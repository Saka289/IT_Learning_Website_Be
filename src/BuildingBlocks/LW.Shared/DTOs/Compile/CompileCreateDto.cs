﻿namespace LW.Shared.DTOs.Compile;

public class CompileCreateDto
{
    public int language_id { get; set; }
    public string source_code { get; set; }
    public string? stdin { get; set; }
    public string expected_output { get; set; }
}