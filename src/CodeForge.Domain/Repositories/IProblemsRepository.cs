using CodeForge.Domain.Entities;

namespace CodeForge.Domain.Repositories;

public interface IProblemsRepository {
	Task<Problem?> GetByIdAsync(int id);
	Task<(IEnumerable<Problem>?, int count)> GetAllAsync(int pageNumber, int pageSize);
	Task<int> CreateAsync(Problem problem);
	Task UpdateAsync(Problem problem);
	Task DeleteAsync(Problem problem);	
}