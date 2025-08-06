using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Problems.Queries.GetProblemById;

public class GetProblemByIdQuery(int id) : IRequest<ProblemDto> {
	public int Id { get; } = id;
}