namespace LW.Shared.SeedWork;

public class SearchRequestValue : PagingRequestParameters
{
    public string? Value { get; set; }
    
    public int Size { get; set; } = 2000;
}