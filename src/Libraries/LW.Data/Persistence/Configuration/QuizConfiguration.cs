using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class QuizConfiguration: IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.Property(x => x.LessonId).IsRequired(false);
        builder.Property(x => x.TopicId).IsRequired(false);
    }
}