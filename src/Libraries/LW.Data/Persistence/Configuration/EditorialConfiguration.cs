using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LW.Data.Persistence.Configuration;

public class EditorialConfiguration : IEntityTypeConfiguration<Editorial>
{
    public void Configure(EntityTypeBuilder<Editorial> builder)
    {
        builder.HasOne(p => p.Problem)
            .WithOne(e => e.Editorial)
            .HasForeignKey<Editorial>(e => e.ProblemId);
    }
}