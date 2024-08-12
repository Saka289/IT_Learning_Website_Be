namespace LW.Shared.Configurations;

public class UrlBase
{
    public string ClientUrl { get; set; }
    public string Judge0Url { get; set; }
    public string Judge0RequestUrl { get; set; }
    public string? Judge0Host { get; set; }
    public string? Judge0HostValue { get; set; }
    public string? Judge0Key { get; set; }
    public string? Judge0KeyValue { get; set; }
    public bool Judge0IsActive { get; set; } = false;
}