using Codeforge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeforge.Infrastructure.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem> {
	public void Configure(EntityTypeBuilder<Problem> builder) {
		builder.HasMany(p => p.TestCases)
			.WithOne(tc => tc.Problem)
			.HasForeignKey(tc => tc.ProblemId);

		builder.HasMany(p => p.Submissions)
			.WithOne(s => s.Problem)
			.HasForeignKey(s => s.ProblemId);

		builder.HasMany(p => p.Tags)
			.WithMany(t => t.Problems);
	}
}