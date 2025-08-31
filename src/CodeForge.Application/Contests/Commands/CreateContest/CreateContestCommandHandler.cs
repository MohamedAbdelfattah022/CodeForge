using Codeforge.Application.Contests.Services;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Contests.Commands.CreateContest;

public class CreateContestCommandHandler(
	ILogger<CreateContestCommandHandler> logger,
	IContestsRepository contestsRepository,
	IStandingsRepository standingsRepository,
	IStandingUpdateService standingUpdateService) : IRequestHandler<CreateContestCommand, int> {
	public async Task<int> Handle(CreateContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("CreateContestCommandHandler.Handle called with request: {@Request}", request);

		var contest = request.ToContest();
		var contestId = await contestsRepository.CreateAsync(contest);
		
		// var standing = new Standing() {
		// 	ContestId = contestId
		// };
		//
		// await standingsRepository.CreateAsync(standing);
		
		standingUpdateService.ScheduleStandingsUpdate(contest.StartTime, contest.EndTime, contestId, $"contest-{contestId}-standing-update");
		
		return contestId;
	}
}