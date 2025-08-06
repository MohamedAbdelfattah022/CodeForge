using Codeforge.Application.Dtos;
using Codeforge.Application.Shared;
using MediatR;

namespace Codeforge.Application.Problems.Queries.GetAllProblems;

public class GetProblemsQuery : IRequest<PaginationResult<ProblemDto>> {
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 5;
}