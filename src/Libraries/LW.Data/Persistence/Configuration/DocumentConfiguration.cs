using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasOne(d => d.Grade)
            .WithMany(d => d.Documents)
            .HasForeignKey(d => d.GradeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}