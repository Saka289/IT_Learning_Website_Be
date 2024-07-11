using LW.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace LW.Data.Persistence.Configuration;

public class UserQuizConfiguration : IEntityTypeConfiguration<UserQuiz>
{
    public void Configure(EntityTypeBuilder<UserQuiz> builder)
    {
        builder.Property(uq => uq.HistoryQuizzes)
            .HasConversion(
                c => JsonConvert.SerializeObject(c),
                c => JsonConvert.DeserializeObject<List<HistoryQuiz>>(c) ?? new List<HistoryQuiz>())
            .IsRequired(false)
            .HasColumnType("json");
        // .Metadata.SetValueComparer(new ValueComparer<List<HistoryQuiz>>(
        //     (c1, c2) => c1.SequenceEqual(c2),
        //     c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        //     c => c.ToList()));
    }
}