using Codeforge.Domain.Entities;

namespace Codeforge.Domain.Repositories;

public interface IContestsRepository : IBaseRepository<Contest> {
	Task<List<Standing>> GetStandingsAsync(int contestId);
}