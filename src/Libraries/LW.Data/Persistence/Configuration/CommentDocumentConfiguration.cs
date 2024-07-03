using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class CommentDocumentConfiguration : IEntityTypeConfiguration<CommentDocument>
{
    public void Configure(EntityTypeBuilder<CommentDocument> builder)
    {
        builder.HasOne(c => c.Document)
            .WithMany(c => c.CommentDocuments)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.ChildCommentDocuments)
            .HasForeignKey(c => c.ParentId).IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}