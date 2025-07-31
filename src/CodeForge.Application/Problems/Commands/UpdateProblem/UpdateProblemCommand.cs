using CodeForge.Domain.Constants;
using MediatR;

namespace CodeForge.Application.Problems.Commands.UpdateProblem;

public class UpdateProblemCommand : IRequest {
	public int Id { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Constraints { get; set; }
	public Difficulty? Difficulty { get; set; }
}