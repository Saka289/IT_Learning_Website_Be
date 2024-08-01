using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class PostCommentConfiguration:IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.HasOne(t => t.PostCommentParent)
            .WithMany(t => t.PostCommentChilds)
            .HasForeignKey(t => t.ParentId).IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Post)
            .WithMany(t => t.PostComments)
            .HasForeignKey(t => t.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
