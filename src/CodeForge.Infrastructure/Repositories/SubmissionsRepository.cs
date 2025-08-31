using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class SubmissionsRepository(CodeforgeDbContext dbContext) : BaseRepository<Submission>(dbContext), ISubmissionsRepository {
	private readonly DbSet<Submission> _dbSet = dbContext.Set<Submission>();

	public async Task<List<Submission>> GetAllSubmissions(int problemId) {
		var data = await _dbSet
			.AsNoTracking()
			.Where(s => s.ProblemId == problemId)
			.ToListAsync();

		return data;
	}

	public async Task<List<Submission>> GetUserSubmissionsAsync(string userId) {
		var data = await _dbSet
			.AsNoTracking()
			.Where(s => s.UserId == userId)
			.ToListAsync();

		return data;
	}

	public async Task<List<Submission>> GetContestSubmissionsForUserAndProblemAsync(int contestId, int problemId, string userId)
	{
		var data = await _dbSet
			.AsNoTracking()
			.Where(s => s.ContestId == contestId && s.ProblemId == problemId && s.UserId == userId)
			.ToListAsync();

		return data;
	}
}