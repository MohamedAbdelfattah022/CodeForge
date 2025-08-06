using MediatR;

namespace Codeforge.Application.Problems.Commands.DeleteProblem;

public class DeleteProblemCommand(int id) : IRequest {
	public int Id { get; } = id;
}