using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class TestcasesRepository(CodeforgeDbContext dbContext) : BaseRepository<TestCase>(dbContext), ITestcasesRepository {
	private readonly DbSet<TestCase> _dbSet = dbContext.Set<TestCase>();

	public async Task<TestCase?> GetProblemTestcaseByIdAsync(int problemId, int testcaseId) {
		return await _dbSet.FirstOrDefaultAsync(tc => tc.ProblemId == problemId && tc.Id == testcaseId);
	}

	public async Task<IEnumerable<TestCase>?> GetProblemTestcasesAsync(int problemId) {
		var data = await _dbSet
			.AsNoTracking()
			.Where(tc => tc.ProblemId == problemId && tc.IsVisible)
			.Take(5)
			.ToListAsync();

		return data;
	}

	public async Task<List<TestCase>?> GetAllProblemTestcasesAsync(int problemId) {
		var data = await _dbSet
			.AsNoTracking()
			.Where(tc => tc.ProblemId == problemId)
			.ToListAsync();

		return data;
	}
}