using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission> {
	public void Configure(EntityTypeBuilder<Submission> builder) {
		builder.Property(s => s.Verdict)
			.HasConversion<string>()
			.HasMaxLength(10);
	}
}