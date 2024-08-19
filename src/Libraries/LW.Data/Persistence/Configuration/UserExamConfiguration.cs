using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace LW.Data.Persistence.Configuration;

public class UserExamConfiguration : IEntityTypeConfiguration<UserExam>
{
    public void Configure(EntityTypeBuilder<UserExam> builder)
    {
        builder.Property(ue => ue.HistoryExam)
            .HasConversion(
                c => JsonConvert.SerializeObject(c),
                c => JsonConvert.DeserializeObject<List<HistoryAnswer>>(c) ?? new List<HistoryAnswer>(),
                new ValueComparer<List<HistoryAnswer>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .IsRequired(false)
            .HasColumnType("json");
    }
}