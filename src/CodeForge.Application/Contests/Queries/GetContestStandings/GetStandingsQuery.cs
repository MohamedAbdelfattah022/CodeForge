using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Contests.Queries.GetContestStandings;

public class GetStandingsQuery(int contestId) : IRequest<List<StandingDto>> {
    public int ContestId { get; } = contestId;
}