using System.ComponentModel.DataAnnotations;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Solution;

public class SearchSolutionDto : SearchRequestValue
{
    [Required]
    public int ProblemId { get; set; }
}