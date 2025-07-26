using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase> {
	public void Configure(EntityTypeBuilder<TestCase> builder) {
		builder.HasMany(tc => tc.SubmissionTestCaseResults)
			.WithOne(sr => sr.TestCase)
			.HasForeignKey(sr => sr.TestCaseId);
	}
}