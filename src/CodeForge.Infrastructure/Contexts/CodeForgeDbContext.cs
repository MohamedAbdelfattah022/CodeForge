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
	public DbSet<Contest> Contests { get; set; }
	public DbSet<Standing> Standings { get; set; }
	public DbSet<ProblemResult> ProblemResults { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new ProblemConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new ContestConfiguration());
		modelBuilder.ApplyConfiguration(new StandingConfiguration());
		modelBuilder.ApplyConfiguration(new ProblemResultConfiguration());
	}
}