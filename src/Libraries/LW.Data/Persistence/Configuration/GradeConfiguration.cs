using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasOne(g => g.Level)
            .WithMany(g => g.Grades)
            .HasForeignKey(g => g.LevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}