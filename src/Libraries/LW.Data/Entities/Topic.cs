﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Topic : EntityAuditBase<int>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string KeyWord { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Objectives { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    [Required]
    public int DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public virtual Document Document { get; set; }
    
    [ForeignKey(nameof(ParentId))]
    public virtual Topic ParentTopic { get; set; }
    public virtual ICollection<Lesson> Lessons { get; set; }
    public virtual ICollection<Topic> ChildTopics { get; set; }
    public virtual ICollection<Quiz> Quizzes { get; set; }
    public virtual ICollection<Problem> Problems { get; set; }
}