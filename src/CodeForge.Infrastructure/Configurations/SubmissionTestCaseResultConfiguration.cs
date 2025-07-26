using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class SubmissionTestCaseResultConfiguration : IEntityTypeConfiguration<SubmissionTestCaseResult> {
	public void Configure(EntityTypeBuilder<SubmissionTestCaseResult> builder) {
		builder.Property(sr => sr.Verdict)
			.HasConversion<string>()
			.HasMaxLength(20);
		
		builder.HasOne(sr => sr.Submission)
			.WithMany(s => s.SubmissionTestCaseResults)
			.HasForeignKey(sr => sr.SubmissionId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasOne(sr => sr.TestCase)
			.WithMany(tc => tc.SubmissionTestCaseResults)
			.HasForeignKey(sr => sr.TestCaseId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}