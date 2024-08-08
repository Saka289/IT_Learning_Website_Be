using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasOne(t => t.Topic)
            .WithMany(t => t.Lessons)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}