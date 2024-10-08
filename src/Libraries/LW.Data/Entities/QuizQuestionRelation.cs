﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class QuizQuestionRelation : EntityBase<int>
{
    [Required]
    public int QuizId { get; set;  }
    [Required]
    public int QuizQuestionId { get; set;  }
    [ForeignKey(nameof(QuizId))]
    public virtual Quiz Quiz { get; set; }
    [ForeignKey(nameof(QuizQuestionId))]
    public virtual QuizQuestion QuizQuestion { get; set; }
}