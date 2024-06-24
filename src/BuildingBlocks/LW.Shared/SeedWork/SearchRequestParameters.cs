using System.ComponentModel.DataAnnotations;

namespace LW.Shared.SeedWork;

public class SearchRequestParameters : PagingRequestParameters
{
    public virtual string? Key { get; set; }
    [Required]
    public string Value { get; set; }
    public int Size { get; set; } = 500;
}