﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LW.Data.Entities;

public class PostComment : EntityAuditBase<int>
{
    [Required]
    public string Content { get; set; }
    [Required]
    public int PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public virtual Post Post { get; set; }
    
    [Required]
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
    
    public int? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual PostComment PostCommentParent { get; set; }
    
    public int? CorrectVote { get; set; }
    
    public ICollection<PostComment> PostCommentChilds { get; set; }
    
    
}