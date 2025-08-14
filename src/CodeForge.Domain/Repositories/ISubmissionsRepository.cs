using Codeforge.Domain.Entities;

namespace Codeforge.Domain.Repositories;

public interface ISubmissionsRepository : IBaseRepository<Submission> {
	Task<List<Submission>> GetAllSubmissons(int problemId);
	Task<List<Submission>> GetUserSubmissionsAsync(string userId);	
}