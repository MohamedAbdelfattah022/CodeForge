using Codeforge.Domain.Entities;

namespace Codeforge.Domain.Repositories;

public interface IProblemsRepository : IBaseRepository<Problem> {
	Task<Problem?> GetByIdWithTagsAsync(int id, bool asTracking = true);
	Task AddTagToProblemAsync(Problem problem, Tag tag);
	Task RemoveTagFromProblemAsync(Problem problem, int tagId);
}