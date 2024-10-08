﻿using LW.Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace LW.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public DateOnly? Dob { get; set; }
    public string? Image { get; set; }
    
    public string? PublicId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<UserGrade> UserGrades { get; set; }
    public virtual ICollection<UserExam> UserExams { get; set; }
    public virtual ICollection<UserQuiz> UserQuizzes { get; set; }
    public virtual ICollection<Submission> Submissions { get; set; }
    public virtual ICollection<VoteComment> VoteComments { get; set; }
    public virtual ICollection<FavoritePost> FavoritePosts { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<PostComment> PostComments { get; set; }
    public virtual ICollection<Solution> Solutions { get; set; }
}