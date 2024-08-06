namespace LW.Shared.DTOs.Compile;

public class CompileDto
{
    public string source_code { get; set; }
    public int language_id { get; set; }
    public string? stdin { get; set; }
    public string? stdout { get; set; }
    public string? expected_output { get; set; }
    public int status_id { get; set; }
    public float? time { get; set; }
    public float? memory { get; set; }
    public string? compile_output { get; set; }
}