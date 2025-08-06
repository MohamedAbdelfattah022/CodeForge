using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Testcases.Queries.GetProblemTestcase;

public class GetProblemTestcaseQuery(int problemId, int testcaseId) : IRequest<TestcaseDto> {
	public int ProblemId { get; set; } = problemId;
	public int TestcaseId { get; set; } = testcaseId;
}