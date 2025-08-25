using Codeforge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeforge.Infrastructure.Configurations;

public class ProblemResultConfiguration : IEntityTypeConfiguration<ProblemResult>
{
    public void Configure(EntityTypeBuilder<ProblemResult> builder)
    {
        builder.HasOne(pr => pr.Standing)
            .WithMany(s => s.Problems)
            .HasForeignKey(pr => pr.StandingId);
    }
}