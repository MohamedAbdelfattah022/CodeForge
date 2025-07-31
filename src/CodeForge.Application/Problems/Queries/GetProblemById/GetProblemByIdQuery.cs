using System.ComponentModel.DataAnnotations;
using CodeForge.Application.Dtos;
using MediatR;

namespace CodeForge.Application.Problems.Queries.GetProblemById;

public class GetProblemByIdQuery(int id) : IRequest<ProblemDto> {
	public int Id { get; } = id;
}