using CodeForge.Domain.Entities;
using CodeForge.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeForge.Infrastructure.Contexts;

public class CodeForgeDbContext(DbContextOptions<CodeForgeDbContext> options) : IdentityDbContext<User>(options) {
	public DbSet<Problem> Problems { get; set; }
	public DbSet<Submission> Submissions { get; set; }
	public DbSet<TestCase> TestCases { get; set; }
	public DbSet<Tag> Tags { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new ProblemConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
	}
}