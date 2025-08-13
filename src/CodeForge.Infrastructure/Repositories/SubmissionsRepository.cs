using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class SubmissionsRepository(CodeforgeDbContext dbContext) : BaseRepository<Submission>(dbContext), ISubmissionsRepository {
	private readonly DbSet<Submission> _dbSet = dbContext.Set<Submission>();

	public async Task<List<Submission>> GetAllSubmissons(int problemId) {
		var data = await _dbSet
			.AsNoTracking()
			.Where(s => s.ProblemId == problemId)
			.ToListAsync();

		return data;
	}
}