using Codeforge.Application.Dtos;
using Codeforge.Application.Shared;
using MediatR;

namespace Codeforge.Application.Contests.Queries.GetAllContests;

public class GetContestsQuery : IRequest<PaginationResult<ContestDto>> {
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 5;
}