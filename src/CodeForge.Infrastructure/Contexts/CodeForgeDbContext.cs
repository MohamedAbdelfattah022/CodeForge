using Codeforge.Domain.Entities;
using Codeforge.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Contexts;

public class CodeforgeDbContext(DbContextOptions<CodeforgeDbContext> options) : IdentityDbContext<User>(options) {
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