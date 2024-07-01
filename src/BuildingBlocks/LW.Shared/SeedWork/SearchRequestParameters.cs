using System.ComponentModel.DataAnnotations;

namespace LW.Shared.SeedWork;

public class SearchRequestParameters : PagingRequestParameters
{
    public virtual string? Key { get; set; }
    public string? Value { get; set; }
    public int Size { get; set; } = 2000;
}