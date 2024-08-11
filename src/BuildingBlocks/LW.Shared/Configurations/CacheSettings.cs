namespace LW.Shared.Configurations;

public class CacheSettings
{
    public string ConnectionString { get; set; }
    public string Password { get; set; }
    public bool Ssl { get; set; }
    public bool AbortOnConnectFail { get; set; }
}