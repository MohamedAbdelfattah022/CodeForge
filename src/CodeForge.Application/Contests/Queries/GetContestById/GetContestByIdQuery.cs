using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Contests.Queries.GetContestById;

public class GetContestByIdQuery(int id) : IRequest<ContestDto> {
	public int Id { get; } = id;
}