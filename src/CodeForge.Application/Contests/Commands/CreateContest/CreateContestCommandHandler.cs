using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Commands.CreateContest;

public class CreateContestCommandHandler(
	ILogger<CreateContestCommandHandler> logger,
	IContestsRepository contestsRepository) : IRequestHandler<CreateContestCommand, int> {
	public async Task<int> Handle(CreateContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("CreateContestCommandHandler.Handle called with request: {@Request}", request);

		var contest = request.ToContest();
		var contestId = await contestsRepository.CreateAsync(contest);
		return contestId;
	}
}