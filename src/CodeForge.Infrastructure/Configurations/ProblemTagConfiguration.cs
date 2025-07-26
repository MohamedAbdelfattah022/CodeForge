using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class ProblemTagConfiguration : IEntityTypeConfiguration<ProblemTag> {
	public void Configure(EntityTypeBuilder<ProblemTag> builder) {
		builder.HasKey(pt => new { pt.ProblemId, pt.TagId });
	}
}