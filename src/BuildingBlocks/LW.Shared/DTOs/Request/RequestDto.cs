using LW.Shared.Enums;

namespace LW.Shared.DTOs.Request;

public class RequestDto
{
    public EApiType ApiType { get; set; } = EApiType.Get;
    public string Url { get; set; }
    public object Data { get; set; }
    public string AccessToken { get; set; }
    public EContentType ContentType { get; set; } = EContentType.Json;
}