using MediatR;

namespace Codeforge.Application.Contests.Commands.DeleteContest;

public class DeleteContestCommand(int id) : IRequest {
	public int Id { get; } = id;
}