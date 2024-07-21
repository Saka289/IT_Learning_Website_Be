using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.HasOne(t => t.ParentTopic)
            .WithMany(t => t.ChildTopics)
            .HasForeignKey(t => t.ParentId).IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Document)
            .WithMany(t => t.Topics)
            .HasForeignKey(t => t.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}