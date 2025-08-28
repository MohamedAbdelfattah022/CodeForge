using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class ContestsRepository(CodeforgeDbContext dbContext) : BaseRepository<Contest>(dbContext), IContestsRepository {
	private readonly CodeforgeDbContext _dbContext = dbContext;
	private readonly DbSet<Contest> _dbSet = dbContext.Set<Contest>();

	public async Task<List<Standing>> GetStandingsAsync(int contestId) {
		return await _dbSet.Where(c => c.Id == contestId)
			.SelectMany(s => s.Standings).ToListAsync();
	}
}