using MediatR;

namespace Codeforge.Application.Tags.Commands.RemoveTagFromProblem;

public class RemoveTagFromProblemCommand : IRequest {
	public int ProblemId { get; set; }
	public int TagId { get; set; }
}