using Codeforge.Domain.Entities;

namespace Codeforge.Domain.Repositories;

public interface ISubmissionsRepository : IBaseRepository<Submission> {
	Task<List<Submission>> GetAllSubmissions(int problemId);
	Task<List<Submission>> GetUserSubmissionsAsync(string userId);
	Task<List<Submission>> GetContestSubmissionsForUserAndProblemAsync(int contestId, int problemId, string userId);
}