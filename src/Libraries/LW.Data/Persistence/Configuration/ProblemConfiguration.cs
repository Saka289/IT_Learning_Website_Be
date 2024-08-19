using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.Property(x => x.LessonId).IsRequired(false);
        builder.Property(x => x.TopicId).IsRequired(false);
        builder.Property(x => x.GradeId).IsRequired(false);
        
        builder.HasOne(p => p.Topic)
            .WithMany(t => t.Problems)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(p => p.Lesson)
            .WithMany(t => t.Problems)
            .HasForeignKey(t => t.LessonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}