using MediatR;

namespace Codeforge.Application.Contests.Commands.UnregisterFromContest;

public class UnregisterFromContestCommand(int contestId) : IRequest {
	public int ContestId { get; } = contestId;
}