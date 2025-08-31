using Codeforge.Domain.Entities;

namespace Codeforge.Domain.Repositories;

public interface IStandingsRepository : IBaseRepository<Standing> {
	public Task<Standing?> GetUserStandings(int contestId, string userId);
}