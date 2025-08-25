using Codeforge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeforge.Infrastructure.Configurations;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.HasMany(c => c.Problems)
            .WithOne(p => p.Contest)
            .HasForeignKey(p=> p.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Participants)
            .WithMany(u => u.Contests)
            .UsingEntity(j=> j.ToTable("ContestParticipants"));
    }
}