using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class ProblemsRepository(CodeforgeDbContext dbContext) : BaseRepository<Problem>(dbContext), IProblemsRepository {
	private readonly DbSet<Problem> _dbSet = dbContext.Set<Problem>();

	public override async Task<Problem?> GetByIdAsync(int id) {
		var result = await _dbSet
			.AsNoTracking()
			.Include(p => p.Tags)
			.Include(p => p.Submissions)
			.Include(p => p.TestCases)
			.FirstOrDefaultAsync(p => p.Id == id);

		return result;
	}

	public override async Task<(IEnumerable<Problem>?, int count)> GetAllAsync(int pageNumber, int pageSize) {
		var data = await _dbSet
			.AsNoTracking()
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var count = await _dbSet.CountAsync();
		return (data, count);
	}
}