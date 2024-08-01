using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.Enums;

namespace LW.Data.Entities;

public class Problem : EntityAuditBase<int>
{
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public EDifficulty Difficulty { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    [NotMapped]
    public EStatusProblem Status { get; set; }
    [ForeignKey(nameof(TopicId))]
    public virtual Topic Topic { get; set; }
    [ForeignKey(nameof(LessonId))]
    public virtual Lesson Lesson { get; set; }
    public virtual Editorial Editorial { get; set; }
    public virtual ICollection<Submission> Submissions { get; set; }
    public virtual ICollection<TestCase> TestCases { get; set; }
    public virtual ICollection<ExecuteCode> ExecuteCodes { get; set; }
    public virtual ICollection<Solution> Solutions { get; set; }
}