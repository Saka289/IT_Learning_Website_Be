﻿namespace LW.Shared.DTOs.Topic;

public class ChildTopicDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string KeyWord { get; set; }
    public string Objectives { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public string DocumentTitle { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}