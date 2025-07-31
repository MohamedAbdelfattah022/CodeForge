using MediatR;

namespace CodeForge.Application.Problems.Commands.DeleteProblem;

public class DeleteProblemCommand(int id) : IRequest {
	public int Id { get; } = id;
}