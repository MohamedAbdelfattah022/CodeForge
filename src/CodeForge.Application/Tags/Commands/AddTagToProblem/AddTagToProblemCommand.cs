using MediatR;

namespace Codeforge.Application.Tags.Commands.AddTagToProblem;

public class AddTagToProblemCommand : IRequest {
	public int ProblemId { get; set; }
	public int TagId { get; set; }
}