using CodeForge.Domain.Entities;

namespace CodeForge.Domain.Repositories;

public interface ITestcasesRepository : IBaseRepository<TestCase> {
	Task<TestCase?> GetProblemTestcaseByIdAsync(int problemId, int testcaseId);
	Task<IEnumerable<TestCase>?> GetProblemTestcasesAsync(int problemId);
}