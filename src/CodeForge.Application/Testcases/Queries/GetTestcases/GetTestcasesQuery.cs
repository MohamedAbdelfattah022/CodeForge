using CodeForge.Application.Dtos;
using MediatR;

namespace CodeForge.Application.Testcases.Queries.GetTestcases;

public class GetTestcasesQuery(int problemId) : IRequest<IEnumerable<TestcaseDto>> {
	public int ProblemId { get; set; } = problemId;
}