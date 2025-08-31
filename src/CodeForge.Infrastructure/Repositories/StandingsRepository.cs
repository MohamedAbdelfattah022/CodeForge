using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class StandingsRepository(CodeforgeDbContext dbContext) : BaseRepository<Standing>(dbContext), IStandingsRepository {
	private readonly DbSet<Standing> _dbSet = dbContext.Set<Standing>();

	public async Task<Standing?> GetUserStandings(int contestId, string userName) {
		return await _dbSet
			.FirstOrDefaultAsync(s => s.ContestId == contestId && s.UserName == userName);
	}
}