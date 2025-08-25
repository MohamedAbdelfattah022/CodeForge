using Codeforge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeforge.Infrastructure.Configurations;

public class StandingConfiguration : IEntityTypeConfiguration<Standing>
{
    public void Configure(EntityTypeBuilder<Standing> builder)
    {
        builder.HasOne(s => s.Contest)
            .WithMany(c => c.Standings)
            .HasForeignKey(s => s.ContestId);

        builder.HasMany(s => s.Problems)
            .WithOne(pr => pr.Standing)
            .HasForeignKey(pr => pr.StandingId);
    }
}