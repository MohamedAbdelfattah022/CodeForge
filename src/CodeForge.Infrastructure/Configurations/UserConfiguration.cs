using Codeforge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeforge.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User> {
	public void Configure(EntityTypeBuilder<User> builder) {
		builder.HasMany(u => u.Submissions)
			.WithOne(s => s.User)
			.HasForeignKey(s => s.UserId);
	}
}