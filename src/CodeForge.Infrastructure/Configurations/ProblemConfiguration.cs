using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem> {
	public void Configure(EntityTypeBuilder<Problem> builder) {
		builder.Property(p => p.Difficulty)
			.HasConversion<string>()
			.HasMaxLength(10);

		builder.HasMany(p => p.TestCases)
			.WithOne(tc => tc.Problem)
			.HasForeignKey(tc => tc.ProblemId);

		builder.HasMany(p => p.Submissions)
			.WithOne(s => s.Problem)
			.HasForeignKey(s => s.ProblemId);

		builder.HasMany(p => p.ProblemTags)
			.WithOne(pt => pt.Problem)
			.HasForeignKey(pt => pt.ProblemId);
	}
}