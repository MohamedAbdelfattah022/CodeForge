using CodeForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeForge.Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag> {
	public void Configure(EntityTypeBuilder<Tag> builder) {
		builder.HasMany(t => t.ProblemTags)
			.WithOne(pt => pt.Tag)
			.HasForeignKey(pt => pt.TagId);
	}
}