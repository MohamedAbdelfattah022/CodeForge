using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Commands.UpdateContest;

public class UpdateContestCommandHandler(
	ILogger<UpdateContestCommandHandler> logger,
	IContestsRepository contestsRepository) : IRequestHandler<UpdateContestCommand> {
	public async Task Handle(UpdateContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("UpdateContestCommandHandler.Handle called with request: {@Request}", request);
		
		if (request.Id <= 0) throw new ValidationException("ID must be Greater than 0.");

		var contest = await contestsRepository.GetByIdAsync(request.Id);
		if (contest is null) throw new NotFoundException(nameof(Contest), request.Id.ToString());

		contest.Name = request.Name ?? contest.Name;
		contest.Description = request.Description ?? contest.Description;
		contest.StartTime = request.StartTime ?? contest.StartTime;
		contest.EndTime = request.EndTime ?? contest.EndTime;
		
		await contestsRepository.UpdateAsync(contest);
	}
}