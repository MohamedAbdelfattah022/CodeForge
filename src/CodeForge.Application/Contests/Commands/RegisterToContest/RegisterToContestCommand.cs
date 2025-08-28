using MediatR;

namespace Codeforge.Application.Contests.Commands.RegisterToContest;

public class RegisterToContestCommand(int contestId) : IRequest {
	public int ContestId { get; } = contestId;
}